using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using DG.Tweening;

namespace UniRx
{


    public class Signal<T>
    {
        public Subject<T> Value { get; }

        public Signal()
        {
            Value = new Subject<T>();
        }
    }  

    public static class UniRxExtensions
    {
        public static T AddTo<T>(this T disposable, CleanRx.Entity entity) where T : IDisposable
        {
            entity.OnDestroy += () => disposable.Dispose();
            return disposable;
        }


        public static IObservable<T> Delay<T> (this IObservable<T> source, Func<TimeSpan> span) 
        {
            return Observable.Create<T>((observer) => 
            {
                CompositeDisposable handlers = new CompositeDisposable();
                source.Subscribe(value => 
                {
                    Observable.Timer(span()).Subscribe(_ => observer.OnNext(value)).AddTo(handlers);
                }).AddTo(handlers);
                return Disposable.Create(() => handlers.Clear());
            });
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, Func<T, TimeSpan> span)
        {
            return Observable.Create<T>((observer) =>
            {
                CompositeDisposable handlers = new CompositeDisposable();
                source.Subscribe(value =>
                {
                    Observable.Timer(span(value)).Subscribe(_ => observer.OnNext(value)).AddTo(handlers);
                }).AddTo(handlers);
                return Disposable.Create(() => handlers.Clear());
            });
        }

        public static IObservable<Tweener> TweenAsObservable(this Tweener tweener)
        {
            return Observable.Create<Tweener>((observer) =>
            {
                tweener.OnComplete(() => observer.OnCompleted());
                return Disposable.Create(() => { });
            });
        }

        public static IObservable<bool> IsPlaying(this Tweener tweener)
        {
            return Observable.Create<bool>((observer) =>
            {
                tweener.OnPlay(() => observer.OnNext(true));
                tweener.OnComplete(() => observer.OnNext(false));
                return Disposable.Create(() => { });
            });
        }
    }
}
