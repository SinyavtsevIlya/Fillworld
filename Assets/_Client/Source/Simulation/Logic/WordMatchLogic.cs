using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UniRx;
using CleanRx;
using UnityEngine;

namespace FillWorld
{
    public class WordMatchLogic : IInitializable
    {
        const float BaseMatchDelay = .05f;
        const float MatchDelayStep = .2f;

        [Inject] readonly CharSequence _combo;
        [Inject] readonly ComboResult _comboResult;
        [Inject] readonly CursorDown _cursorDown;
        [Inject] readonly CompositeDisposable _lifecycle;

        [Inject] void Construct(CleanRx.Context context) => _actualActors = context.GetGroup()
            .With<Name>()
            .Without<Matched>();

        Group _actualActors;

        public void Initialize()
        {
            _cursorDown
                .Where(isTrue => !isTrue)
                .SelectMany(_ => GetMatchedActors(_combo))
                .Delay(actor => TimeSpan.FromSeconds(BaseMatchDelay + MatchDelayStep
                * (actor.Has<OrderIndex>() ? actor.Get<OrderIndex>().Value : 0f)))
                .Subscribe(actor => actor.Add<Matched>())
                .AddTo(_lifecycle);

            _cursorDown
                .Where(isTrue => !isTrue)
                .Select(_ => GetMatchedActors(_combo).Count() > 0)
                .Where(__ => _combo.Count > 0)
                .Subscribe(isMatched => 
                {
                     _comboResult.Value.OnNext(isMatched);
                }).AddTo(_lifecycle);
        }

        IEnumerable<Entity> GetMatchedActors(CharSequence combo)
        {
            var comboName = new string(combo.Select(c => c.Get<Letter>().Value).ToArray());
            return _actualActors.Where(actor => actor.Get<Name>().Value == comboName);
        }
    }
}
 