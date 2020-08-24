using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class ValidateComboLogic : IInitializable
    {
        [Inject] readonly CharSequence _combo;
        [Inject] readonly LetterSelected _letterSelected;
        [Inject] readonly CompositeDisposable _lifecycle;

        public void Initialize()
        {
            _letterSelected.Subscribe(letter => 
            {
                if (!_combo.Contains(letter))
                {
                    _combo.Add(letter);
                }
                else
                {
                    if (_combo.Count > 1)
                    {
                        var secondToLastLetter = _combo[_combo.Count - 2];
                        if (letter == secondToLastLetter)
                        {
                            _combo.Remove(_combo.Last());
                        }
                    }
                }
            }).AddTo(_lifecycle);

            //TODO: move to logic

            _combo.ObserveAdd().Subscribe(letter => letter.Value.Add<Active>()).AddTo(_lifecycle);
            _combo.ObserveRemove().Subscribe(letter => letter.Value.Remove<Active>()).AddTo(_lifecycle);
            _combo.ObserveBeforeReset().Subscribe(_ => 
            {
                foreach (var letter in _combo) letter.Remove<Active>();
            }).AddTo(_lifecycle);

        }
    }
}
