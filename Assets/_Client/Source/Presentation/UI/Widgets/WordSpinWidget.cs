using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;
using DG.Tweening;

namespace FillWorld
{
    public class WordSpinWidget : PanelPresenter, IInitializable, IDisposable
    {
        [Inject] readonly CharPattern _pattern;
        [Inject] readonly CharSequence _combo;
        [Inject] readonly Position2D _cursorPosition;
        [Inject] readonly CursorDown _cursorDown;
        [Inject] readonly Entity _level;

        [SerializeField] RadialLayout _layout;
        [SerializeField] Transform _linesRoot;
        [SerializeField] Color _lineColor;
        [SerializeField] Sprite _cursorSprite;
        [SerializeField] Sprite _connectionSprite;
        [SerializeField] TweenData _connection;
        [SerializeField] float _connectionOffset;
        [SerializeField] float _connectionWidthMultiplier;

        #region State
        UILine _pointerLine;
        IDisposable _pointerLineMovement;
        #endregion

        public void Dispose()
        {
            _pointerLineMovement.Dispose();
            _pointerLine.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            _pattern.ObserveAdd()
                .Subscribe(CreateLetter)
                .AddTo(this);

            _combo.ObserveAdd()
                .Where(_ => _combo.Count > 1)
                .Subscribe(CreateConnection)
                .AddTo(this);

            HandleCursorConnection();
        }

        #region Binding
        void HandleCursorConnection()
        {
            _pointerLine = CreateLine();
            _pointerLine.type = UnityEngine.UI.Image.Type.Sliced;
            _pointerLine.sprite = _cursorSprite;
            _pointerLineMovement = Observable.EveryUpdate()
                .Select(_ => _cursorPosition.Value)
                .Where(_ => _combo.Count > 0)
                .Subscribe(position =>
                {
                    var lastLetterPosition = GetPresenter<LetterWidget>(_combo.Last()).transform.localPosition;
                    var cursorPos = (position - (Vector2)transform.parent.position) / Canvas.scaleFactor;

                    var offsetNormalized = ((Vector3)cursorPos - lastLetterPosition).normalized;

                    lastLetterPosition += offsetNormalized * _connectionOffset;

                    _pointerLine.SetPositions(cursorPos, lastLetterPosition);
                }).AddTo(this);

            _combo.ObserveCountChanged().Select(count => count > 0).Subscribe(value => _pointerLine.gameObject.SetActive(value)).AddTo(this);
        }

        void CreateLetter(CollectionAddEvent<Entity> letter)
        {
            var widget = Create<LetterWidget>()
                .OfBindedModel(letter.Value)
                .UnderTransform(_layout.transform)
                .Release();

            letter.Value.OnDestroy += () =>
            {
                UnbindPresenter(letter.Value);
                widget.Destroy();
            };
        }

        void CreateConnection(CollectionAddEvent<Entity> letter)
        {
            var currentPosition = GetPresenter<LetterWidget>(letter.Value).transform.localPosition;
            var prevPosition = GetPresenter<LetterWidget>(_combo[_combo.Count - 2]).transform.localPosition;

            var offsetNormalized = (currentPosition - prevPosition).normalized;

            currentPosition -= offsetNormalized * _connectionOffset;
            prevPosition += offsetNormalized * _connectionOffset;

            var line = CreateLine();
            line.sprite = _connectionSprite;
            line.transform.localScale = new Vector3(1f, 0f, 1f);
            _connection.Tweener = line.transform.DOScaleY(1f, _connection.Duration).SetEase(_connection.Ease);
            var lineGO = line.gameObject;
            line.SetPositions(prevPosition, currentPosition);
            letter.Value.HasAsObservable<Active>().Where(isTrue => !isTrue).Subscribe(_ => { if (lineGO != null) Destroy(lineGO); }).AddTo(this);
            letter.Value.OnDestroy += () =>
            {
                if (lineGO != null) Destroy(lineGO);
            };
        }

        UILine CreateLine()
        {
            var lineGO = new GameObject("line", typeof(RectTransform));
            lineGO.transform.SetParent(_linesRoot);
            var line = lineGO.AddComponent<UILine>();
            line.SetWidth(UnityEngine.Random.Range(35f, 55f));
            line.color = _lineColor;
            return line;
        } 
        #endregion
    }
}
