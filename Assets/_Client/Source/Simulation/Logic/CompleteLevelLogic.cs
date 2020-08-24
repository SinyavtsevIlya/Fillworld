using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class CompleteLevelLogic : IInitializable
    {
        [Inject] readonly LevelIndex _levelIndex;
        [Inject] readonly CompositeDisposable _lifecycle;

        Group _actors;
        Group _matchedActors;

        [Inject] void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup()
                .With<Name>()
                .Without<IsExtra>();

            _matchedActors = context.GetGroup()
                .With<Name, Matched>()
                .Without<IsExtra>();
        }

        public void Initialize()
        {
            _matchedActors.ObserveAdd
                .Where(_ => _matchedActors.Count == _actors.Count)
                .Throttle(TimeSpan.FromMilliseconds(1000))
                .Subscribe(actor => _levelIndex.Value++)
                .AddTo(_lifecycle);
        }
    }
}
