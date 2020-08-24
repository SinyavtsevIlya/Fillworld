using System;
using UniRx;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public abstract class PanelPresenter : MonoBehaviour
{
    [Inject] readonly UIFactory uiFactory;

    public PanelsRegistry Registry = new PanelsRegistry();

    new bool enabled;
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            if (enabled == value) return;

            enabled = value;
            gameObject.SetActive(value);
            if (value)
            {
                if (OnEnableInternal != null) OnEnableInternal(this);
                _canvas = GetComponentInParent<UnityEngine.Canvas>();
                OnShow();
            }
            else
            {
                if (OnDisableInternal != null) OnDisableInternal(this);
                disposables.Visibility.Clear();
                OnHide();
            }
        }
    }

    protected void UnbindPresenter(object model)
    {
        var key = model.GetHashCode();
        var presenter = Registry.presenterByModel[key];
        Registry.presenterByModel.Remove(key);
        Registry.modelByPresenter.Remove(presenter.GetHashCode());
    }

    protected PanelBuilder<T> Create<T>() where T : PanelPresenter
    {
        return new PanelBuilder<T>(uiFactory, this);
    }

    protected T GetPresenter<T>(object model) where T : PanelPresenter
    {
        return Registry.GetPresenter<T>(model);
    }

    protected T GetModel<T>(object presenter) where T : class
    {
        return Registry.GetModel<T>(presenter);
    }

    Action<PanelPresenter> OnEnableInternal;
    Action<PanelPresenter> OnDisableInternal;

    protected PanelDisposables disposables = new PanelDisposables();

    bool isDisposed;

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    public virtual void HandleOptionalParameters(object[] p) { }

    protected UnityEngine.Canvas Canvas => _canvas;
    UnityEngine.Canvas _canvas;
    
    protected UnityEngine.UI.CanvasScaler CanvasScaler
    {
        get
        {
            return GetComponentInParent<UnityEngine.UI.CanvasScaler>();
        }
    }

    public void InternalInitialize(Action<PanelPresenter> onEnable, Action<PanelPresenter> onDisable)
    {
        OnEnableInternal = onEnable;
        OnDisableInternal = onDisable;
    }

    public void Destroy()
    {
        if (OnDisableInternal != null) OnDisableInternal(this);
        TryDispose();
        if (gameObject != null) GameObject.Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        TryDispose();
    }

    void TryDispose()
    {
        if (!disposables.Visibility.IsDisposed) disposables.Visibility.Clear();
        if (!disposables.Lifecycle.IsDisposed) disposables.Lifecycle.Clear();
        isDisposed = true;
    }

    public class PanelDisposables
    {
        public CompositeDisposable Lifecycle = new CompositeDisposable();
        public CompositeDisposable Visibility = new CompositeDisposable();
    }

    public class PanelsRegistry
    {
        public Dictionary<int, object> modelByPresenter = new Dictionary<int, object>();
        public Dictionary<int, object> presenterByModel = new Dictionary<int, object>();

        public T GetPresenter<T>(object model) where T : PanelPresenter
        {
            object presenter;
            if (presenterByModel.TryGetValue(model.GetHashCode(), out presenter))
            {
                return presenter as T;
            }
            return null;
        }

        public T GetModel<T>(object presenter) where T : class
        {
            object model;
            if (modelByPresenter.TryGetValue(presenter.GetHashCode(), out model))
            {
                return model as T;
            }
            return null;
        }
    }
}
