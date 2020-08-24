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
    public class ComboWidget : PanelPresenter, IInitializable
    {
        [Inject] readonly CharSequence _combo;
        [Inject] readonly ComboResult _comboResult;
        [Inject] readonly Image _progressImage;
        [Inject] readonly Transform _extraWordsTr;

        Group _matchedActors;
        [Inject] void GetGroups(CleanRx.Context context) => _matchedActors = context.GetGroup().With<Name, Matched>();

        [SerializeField] TMPro.TMP_Text _label;
        [SerializeField] GameObject _back;

        [SerializeField] TweenData _matchFlow;
        [SerializeField] ShakeTweenData _missMatchShake;
        [SerializeField] Color _matchColor;

        public void Initialize()
        {
            _combo.ObserveCountChanged(true)
                .Where(c => c > 0)
                .Select(_ => new string(_combo.Select(c => c.Get<Letter>().Value).ToArray()))
                .Subscribe(value => 
                {
                    _label.text = value;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_label.transform.parent.GetComponent<RectTransform>());
                })
                .AddTo(this);

            _comboResult.Value.Where(isTrue => !isTrue)
                .Subscribe(_ =>
                {
                    var tr = _back.transform;
                    var shake = _missMatchShake;

                    shake.Tweener?.Kill();
                    shake.Tweener = tr.DOShakePosition(shake.Duration, Vector3.right * shake.Strength, shake.Vibrato, 0).OnComplete(() => shake.Tweener = null);
                })
                .AddTo(this);

            var isShakePlaying = this.ObserveEveryValueChanged(_ => _missMatchShake.Tweener)
                .Select(tweener => tweener != null)
                .ToReadOnlyReactiveProperty();

            var isComboNotEmpty =_combo.ObserveCountChanged(true)
                .Select(c => c > 0)
                .ToReadOnlyReactiveProperty();

            //Observable.Merge(isComboNotEmpty, isShakePlaying)
            //    .Subscribe(value => 
            //    {
            //        _back.SetActive(value);
            //        LayoutRebuilder.ForceRebuildLayoutImmediate(_label.transform.parent.GetComponent<RectTransform>());
            //    })
            //    .AddTo(this);

            Observable.EveryUpdate()
                .Select(_ => 
                {
                    if (isComboNotEmpty.Value) return true;
                    if (!isComboNotEmpty.Value && !isShakePlaying.Value) return false;
                    if (!isComboNotEmpty.Value && isShakePlaying.Value) return true;
                    return true;
                })
                .Subscribe(value =>
                {
                    _back.SetActive(value);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_label.transform.parent.GetComponent<RectTransform>());
                })
                .AddTo(this);

            _matchedActors.ObserveAdd.Subscribe(actor => 
            {
                for (int i = 0; i < _label.text.Length; i++)
                {
                    char letter = _label.text[i];
                    var charInfo = _label.textInfo.characterInfo[i];
                    var instance = GameObject.Instantiate<GameObject>(_label.gameObject, transform);
                    instance.SetActive(true);
                    var tmp = instance.GetComponent<TMPro.TMP_Text>();
                    tmp.text = letter.ToString();
                    tmp.DOColor(_matchColor, _matchFlow.Duration).SetDelay(_matchFlow.Delay * i);

                    instance.transform.position = _label.transform.position + GetAverage(charInfo.bottomLeft, charInfo.bottomRight, charInfo.topLeft, charInfo.topRight);

                    var targetPosition = GetTargetPosition(actor.Has<IsExtra>());

                    Observable.Timer(TimeSpan.FromSeconds(_matchFlow.Delay * i))
                    .Subscribe(__ =>
                    {
                        var tweenObservable = instance.transform
                            .DOMove(targetPosition, _matchFlow.Duration)
                            .SetEase(_matchFlow.Ease)
                            .OnComplete(() => Destroy(instance));
                    }).AddTo(this);
                }
            });
        }

        Vector3 GetTargetPosition(bool isExtra)
        {
            Vector3 targetPosition;

            if (isExtra)
            {
                targetPosition = _extraWordsTr.position;
            }
            else
            {
                var rect = GetWorldRect(_progressImage.rectTransform, Vector2.one);
                targetPosition = rect.min + (rect.max - rect.min) * _progressImage.fillAmount;
            }

            return targetPosition;
        }

        Vector3 GetAverage(params Vector3[] input)
        {
            var count = input.Length;
            var x = input.Select(v => v.x).Sum() / count;
            var y = input.Select(v => v.y).Sum() / count;
            var z = input.Select(v => v.z).Sum() / count;
            return new Vector3(x, y, z);
        }

        Rect GetWorldRect(RectTransform rt, Vector2 scale)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
            return new Rect(topLeft, scaledSize);
        }
    }
}

