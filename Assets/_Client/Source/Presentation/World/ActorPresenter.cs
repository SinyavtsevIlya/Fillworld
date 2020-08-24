using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;
using DG.Tweening;

namespace FillWorld
{
    public class ActorPresenter : MonoBehaviour, IInitializable
    {
        [Inject] Entity _actor;
        [Inject] ActorViewSettings _actorViewSettings;

        Tweener hintTweener;
        Tweener matchTweener;

        public void Initialize()
        {
            var match = _actorViewSettings.Match;
            var hint = _actorViewSettings.Hint;
            var explosionPrefab = _actorViewSettings.ExplosionPS;
            var actorMR = _actor.Get<View>().Value.GetComponentInChildren<MeshRenderer>();

            var hintDisposable = _actor.HasAsObservable<Hinted>()
                .Where(isTrue => isTrue)
                .Subscribe(value =>
                {
                    if (value)
                    {
                        hintTweener = transform.DOPunchScale(hint.Strength * Vector3.one, hint.Duration, hint.Vibrato).SetLoops(-1);
                    }
                    else
                    {
                        hintTweener?.Kill();
                    }
                });

            _actor.HasAsObservable<Matched>()
                .Where(isTrue => isTrue)
                .Subscribe(_ => 
                {
                    hintDisposable?.Dispose();
                    hintTweener?.Kill();

                    matchTweener = transform.DOScale(Vector3.zero, match.Duration).SetEase(match.Ease);

                    if (actorMR == null) return;

                    var go = GameObject.Instantiate<GameObject>(explosionPrefab, _actor.Get<Position>().Value, Quaternion.identity);
                    go.transform.localScale = Vector3.one * _actorViewSettings.ExplosionScaleMultiplier * actorMR.bounds.size.magnitude;

                    var ps = go.GetComponent<ParticleSystem>();
                    Observable.Timer(System.TimeSpan.FromSeconds(match.Duration / 2f)).Subscribe(___ => ps.Play()).AddTo(this);
                    var shapeModule = ps.shape;
                    shapeModule.meshRenderer = actorMR;
                    actorMR.material = _actorViewSettings.MatchMateril;
                }).AddTo(this);
        }
    }
}
