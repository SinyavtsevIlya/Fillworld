using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public class UIService
{
    #region Dependencies
    UIFactory _uiFactory;
    #endregion

    #region State
    bool _isInitialized;
    List<WindowPresenter> _windows;
    WindowPresenter _currentWindow;
    CompositeDisposable _disposables;
    Canvas _canvas;
    #endregion

    #region Constructor
    public UIService(UIFactory uIFactory)
    {
        _uiFactory = uIFactory;
    } 
    #endregion

    #region API
    public UIService Initialize<T>() where T : WindowPresenter 
    {
        _canvas = _uiFactory.CreateCanvas();
        _windows = _uiFactory.CreateWindows();

        _disposables = new CompositeDisposable();

        _currentWindow = _windows.Find(x => x is T);
        _currentWindow.Enabled = true;

        _isInitialized = true;

        RefreshLayout(Screen.orientation);

        Observable.EveryUpdate().Select(_ => Screen.orientation)
            .ToReactiveProperty()
            .Subscribe(orientation =>
            {
                RefreshLayout(orientation);
            })
            .AddTo(_disposables);

        void RefreshLayout(ScreenOrientation orientation)
        {
            foreach (var window in _windows)
            {
                window.SafeAreaOffset = GetSafeAreaOffset(_canvas);
                window.OnOrientationChanged(orientation);
            }
        }
        return this;
    }

    public T GetWindow<T>() where T : WindowPresenter
    {
        return _windows.Find(w => w is T) as T;
    }

    public UIService DisplayWindow<T>(params object[] p) where T : WindowPresenter
    {
        DisplayWindowInternal(GetWindow<T>(), p);
        return this;
    }

    public WindowPresenter CurrentWindow => _currentWindow;

    public UIService CloseCurrentWindow()
    {
        CloseCurrentWindowInternal();
        return this;
    }
    #endregion

    #region Internal
    void DisplayWindowInternal(WindowPresenter nextWindow, object[] p = null)
    {
        if (!_isInitialized)
        {
            Debug.LogError(string.Format("Unable to display {0}-window, UIService is not initialized", nextWindow.name));
            return;
        }

        if (nextWindow.WindowType == WindowTypes.Fullscreen)
        {
            _currentWindow.Enabled = false;
        }
        else
        {
            MoveToForeground(nextWindow);
        }

        if (nextWindow.IsDynamic)
        {
            nextWindow.Parent = _currentWindow;
        }

        _currentWindow = nextWindow;

        if (p != null)
        {
            _currentWindow.HandleOptionalParameters(p);
        }

        _currentWindow.Enabled = true;
    }

    void CloseCurrentWindowInternal()
    {
        _currentWindow.Enabled = false;
        var nextWindow = _windows.Find(w => w == _currentWindow.Parent);

        if (_currentWindow.WindowType == WindowTypes.Fullscreen)
        {
            if (_currentWindow.IsRootWindow)
            {
                //TODO: Provide closing app logic
                return;
            }
        }

        if (_currentWindow.WindowType == WindowTypes.Fullscreen || _currentWindow.IsDynamic)
        {
            nextWindow.Enabled = true;
        }

        _currentWindow = nextWindow;
    }

    void MoveToForeground(WindowPresenter Window)
    {
        Window.transform.SetAsLastSibling();
    }

    SafeAreaOffset GetSafeAreaOffset(Canvas canvas)
    {
        var safeAreaRect = Screen.safeArea;
        var scaleRatio = canvas.GetComponent<RectTransform>().rect.width / Screen.width;

        var left = safeAreaRect.xMin * scaleRatio;
        var right = (Screen.width - safeAreaRect.xMax) * scaleRatio;
        var top = (Screen.height - safeAreaRect.yMax) * scaleRatio;
        var bottom = safeAreaRect.yMin * scaleRatio;

        return new SafeAreaOffset(left, right, top, bottom);
    } 
    #endregion
}
