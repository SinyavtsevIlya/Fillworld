using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using CleanRx;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace FillWorld
{
    public class ExtraWordWidget : PanelPresenter, IInitializable
    {
        Group _matchedActors;
        Group _actors;
        [Inject] void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup().With<Name, IsExtra>();
            _matchedActors = context.GetGroup().With<Name, IsExtra, Matched>();
        }

        [SerializeField] TMPro.TMP_Text _label;
        [SerializeField] GameObject _back;

        [SerializeField] ShakeTweenData _match;

        public void Initialize()
        {
            this.ObserveEveryValueChanged(_ => _matchedActors.Count).Subscribe(count => 
            {
                _label.text = $"{count}/{_actors.Count}";
                _back.SetActive(count > 0);

                _match.Tweener?.Kill();
                _match.Tweener = _back.transform.DOPunchScale(_match.Strength * Vector2.one, _match.Duration, _match.Vibrato);
            }).AddTo(this);
        }
    }
}

