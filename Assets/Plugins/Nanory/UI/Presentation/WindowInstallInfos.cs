using System;
using UnityEngine;
using Zenject;

public class WindowInstallInfo
{
    public Type type;
    public object[] parameters;

    public WindowInstallInfo(Type type, object[] parameters)
    {
        this.type = type;
        this.parameters = parameters;
    }
}

public static class WindowInstallExtensions
{
    public static void InstallWindow<T>(this DiContainer container, params object[] parameters) where T : WindowPresenter
    {
        container.BindInstance(new WindowInstallInfo(typeof(T), parameters)).AsCached();
    }
}