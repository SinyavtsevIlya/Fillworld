using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UniRx;
using CleanRx;
using UnityEngine;

namespace FillWorld
{
    public class ResetComboLogic : IInitializable
    {
        [Inject] readonly CharSequence _combo;
        [Inject] readonly CompositeDisposable _lifecycle;
        [Inject] readonly CursorDown _cursorDown;


        public void Initialize()
        {
            _cursorDown
                .Where(isTrue => !isTrue)
                .Subscribe(_ =>
                {
                    _combo.Clear();
                }).AddTo(_lifecycle);
        }
    }
}
