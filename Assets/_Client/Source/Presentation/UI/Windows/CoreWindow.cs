using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class CoreWindow : WindowPresenter, IInitializable
    {
        [Inject] Entity _level;
        [Inject] LevelIndex _levelIndex;
        [Inject] HintRequest _hintRequest;

        [SerializeField] Transform _spinRoot;
        [SerializeField] Transform _progressRoot;
        [SerializeField] Transform _comboRoot;
        [SerializeField] Transform _extraWordsRoot;
        [SerializeField] Transform _hintRoot;
        [SerializeField] Button _hintButton;

        WordSpinWidget _spin;
        LevelProgressWidget _levelProgressWidget;
        ComboWidget _comboWidget;
        ExtraWordWidget _extraWordWidget;
        HintWidget _hintWidget;

        public void Initialize()
        {
            _spin = Create<WordSpinWidget>()
                .UnderTransform(_spinRoot)
                .Release();

            _levelProgressWidget = Create<LevelProgressWidget>()
                .UnderTransform(_progressRoot)
                .Release();
            
            _comboWidget = Create<ComboWidget>()
                .UnderTransform(_comboRoot)
                .WithArguments(_levelProgressWidget.ProgressImage, _extraWordsRoot)
                .Release();

            _extraWordWidget = Create<ExtraWordWidget>()
                .UnderTransform(_extraWordsRoot)
                .Release();

            _hintWidget = Create<HintWidget>()
                .UnderTransform(_hintRoot)
                .Release();

            _levelIndex.Subscribe(value => 
            {

            }).AddTo(this);

            _hintButton
                .OnClickAsObservable()
                .Subscribe(_ => _hintRequest.Value.OnNext(Unit.Default))
                .AddTo(this);
        }
    }
}
