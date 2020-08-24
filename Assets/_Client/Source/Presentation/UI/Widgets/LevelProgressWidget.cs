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
    public class LevelProgressWidget : PanelPresenter, IInitializable
    {
        [Inject] LevelIndex _levelIndex;
        Group _actors;
        Group _matchedActors;
        [Inject] void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup().With<Name>().Without<IsExtra>();
            _matchedActors = context.GetGroup().With<Name, Matched>().Without<IsExtra>();
        }

        [SerializeField] TMPro.TMP_Text _currentLevelLabel;
        [SerializeField] TMPro.TMP_Text _nextLevelLabel;

        [SerializeField] Image _currentLevelImage;
        [SerializeField] Image _nextLevelImage;

        [SerializeField] Image _progressImage;

        [SerializeField] Color _accentColor;
        [SerializeField] Color _backColor;

        [SerializeField] TweenData progression;

        public Image ProgressImage => _progressImage;

        public void Initialize()
        {
            _progressImage.fillAmount = 0f;

            _matchedActors.ObserveCountChange
                .Subscribe(_ =>
                {
                    var value = _actors.Count > 0 ? (float)_matchedActors.Count / _actors.Count : 0;
                    progression.Tweener = _progressImage.DOFillAmount(value, progression.Duration).SetEase(progression.Ease);
                }).AddTo(this);

            _matchedActors.ObserveAdd
                .Where(_ => _matchedActors.Count == _actors.Count)
                .Throttle(TimeSpan.FromSeconds(progression.Duration))
                .Subscribe(_ => _nextLevelImage.color = _accentColor).AddTo(this);

            _levelIndex
                .Select(index => index + 1)
                .Subscribe(humanizedIndex =>
                {
                    _nextLevelImage.color = _backColor;
                    _currentLevelLabel.text = humanizedIndex.ToString();
                    _nextLevelLabel.text = (humanizedIndex + 1).ToString();
                }).AddTo(this);
        }
    }
}
