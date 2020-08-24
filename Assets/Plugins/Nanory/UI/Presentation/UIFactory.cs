using Zenject;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using TMPro;
using UnityEngine.UI;

public class UIFactory
{
    const string WindowsFolder = "Windows/";
    const string WidgetsFolder = "Widgets/";
    const string CanvasesFolder = "Canvases/";

    [Inject] protected string _contextFolder;
    [Inject] protected DiContainer _container;
    [Inject] protected List<WindowInstallInfo> _windowInstallInfos;
    [Inject] protected UIRegistry _uiRegistry;

    Canvas _canvas;
    PanelPresenter[] _widgets;

    public Canvas CreateCanvas()
    {
        _canvas = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(_contextFolder + "/" + CanvasesFolder + "Canvas")).GetComponent<Canvas>();
        return _canvas;
    }

    public T CreateWidget<T>(Transform parent, object[] parameters) where T : PanelPresenter
    {
        var presenter = _widgets.Where(p => p.GetType() == typeof(T)).First();
        var prefab = presenter.gameObject;
        var widget = _container.InstantiatePrefabForComponent<T>(prefab, parent, parameters);
        HandleLifecycle(widget);
        widget.Enabled = true;
        return widget;
    }

    public T InjectWidget<T>(T widget, params object[] parameters)
    {
        throw new NotImplementedException();
    }

    public List<WindowPresenter> CreateWindows()
    {
        var presenters = Resources.LoadAll<WindowPresenter>(_contextFolder + "/" + WindowsFolder);
        _widgets = Resources.LoadAll<PanelPresenter>(_contextFolder + "/" + WidgetsFolder);
        var result = new List<WindowPresenter>();
        foreach (var installInfo in _windowInstallInfos)
        {
            var prefab = presenters.Where(p => p.GetType() == installInfo.type).First().gameObject;
            var window = CreateWindow(prefab, installInfo.parameters);
            result.Add(window);
        }
        return result;
    }

    WindowPresenter CreateWindow(GameObject prefab, params object[] parameters)
    {
        var go = GameObject.Instantiate<GameObject>(prefab, _canvas.transform);
        var presenter = go.GetComponent<WindowPresenter>();
        _container.Inject(presenter, parameters);
        presenter.Activate(false);
        HandleLifecycle(presenter);

        UI_RegistryBehaviour registryBehavior;
        if (go.TryGetComponent(out registryBehavior))
        {
            foreach (var node in registryBehavior.Nodes)
            {
                _uiRegistry.Register(node.Name, node);
            }
        }
        return presenter;
    }

    void HandleLifecycle(PanelPresenter panel)
    {
        if (panel is IInitializable i) i.Initialize();
    }
}