using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using CleanRx;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace FillWorld
{
    public class HintWidget : PanelPresenter, IInitializable
    {
        [SerializeField] TMPro.TMP_Text _label;
        [SerializeField] Transform _back;

        Group _hintedActors;
        [Inject]
        void GetGroups(CleanRx.Context context)
        {
            _hintedActors = context.GetGroup().With<Name, Hinted>().Without<IsExtra, Matched>();
        }

        public void Initialize()
        {
            _hintedActors.ObserveCountChange.ObserveOnMainThread().Where(_ => _hintedActors.Count > 0).Subscribe(_ => 
            {
                _label.text = _hintedActors.Last().Get<Name>().Value;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_label.transform.parent.GetComponent<RectTransform>());
            }).AddTo(this);

            _hintedActors.ObserveCountChange
                .ObserveOnMainThread()
                .Select(_ => _hintedActors.Count != 0)
                .Subscribe(value => _back.gameObject.SetActive(value))
                .AddTo(this);
        }

    }
}

