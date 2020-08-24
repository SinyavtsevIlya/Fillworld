using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PanelBuilder<T> where T : PanelPresenter
{
    readonly UIFactory _uiFactory;
    readonly PanelPresenter _parent;

    public Transform Root;
    public object ID;
    public object Model;
    public object[] ExtraArgs;

    public PanelBuilder(UIFactory uiFactory, PanelPresenter parent)
    {
        _uiFactory = uiFactory;
        _parent = parent;
    }

    public PanelBuilder<T> UnderTransform(Transform transform)
    {
        Root = transform;
        return this;
    }

    public PanelBuilder<T> WithID(object id)
    {
        ID = id;
        return this;
    }

    public PanelBuilder<T> OfBindedModel(object model)
    {
        Model = model;
        return this;
    }

    public PanelBuilder<T> WithArguments(params object[] parameters)
    {
        ExtraArgs = parameters;
        return this;
    }

    public T Release()
    {
        var overallParameters = new List<object>();
        if (Model != null) overallParameters.Add(Model);
        if (ExtraArgs != null) overallParameters.AddRange(ExtraArgs);
        Transform parent = Root == null ? _parent.transform : Root;
        
        var presenter = _uiFactory.CreateWidget<T>(parent, overallParameters.ToArray());

        if (Model != null)
        {
            _parent.Registry.modelByPresenter.Add(presenter.GetHashCode(), Model);
            _parent.Registry.presenterByModel.Add(Model.GetHashCode(), presenter);
        }

        return presenter;
    }
}