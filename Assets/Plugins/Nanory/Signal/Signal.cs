using System;

public class Signal<T> : IDisposable
{
    Action<T> _action;

    public Signal(Action<T> action)
    {
        this._action = action;
    }

    public Signal()
    {
        _action = delegate { };
    }

    public void Invoke(T value)
    {
        _action?.Invoke(value);
    } 

    public SignalSubscription Subscribe(Action<T> value)
    {
        _action += value;
        return new SignalSubscription(() => { if (_action != null) _action -= value; });
    }

    public void Dispose()
    {
        _action = null;
    }
}

public class Signal : IDisposable
{
    public Action _action;

    public Signal(Action action)
    {
        this._action = action;
    }

    public Signal()
    {
        _action = delegate { };
    }

    public void Invoke()
    {
        _action?.Invoke();
    }

    public SignalSubscription Subscribe(Action value)
    {
        _action += value;
        return new SignalSubscription(() => { if (_action != null) _action -= value; });
    }

    public void Dispose()
    {
        _action = null;
    }
}

public class SignalSubscription : IDisposable
{
    readonly Action _onDispose;

    public SignalSubscription(Action onDispose)
    {
        this._onDispose = onDispose;
    }

    public void Dispose()
    {
        _onDispose?.Invoke();
    }
}
