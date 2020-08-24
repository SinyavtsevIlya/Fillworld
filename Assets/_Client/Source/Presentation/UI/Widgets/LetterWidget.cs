using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using CleanRx;
using UnityEngine.EventSystems;

namespace FillWorld
{
    public class LetterWidget : PanelPresenter, IInitializable, IPointerEnterHandler, IPointerDownHandler
    {
        [Inject] Entity _letter;
        [Inject] LetterSelected _selected;
        [Inject] CursorDown _cursorDown;

        [SerializeField] TMPro.TMP_Text _label;
        [SerializeField] Image _selectionImage;

        public void Initialize()
        {
            _label.text = _letter.Get<Letter>().Value.ToString();

            _letter.HasAsObservable<Active>()
                .Subscribe(value =>
                {
                    _selectionImage.gameObject.SetActive(value);
                }).AddTo(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _selected.Invoke(_letter);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_cursorDown.Value)
                _selected.Invoke(_letter);
        }
    }
}
