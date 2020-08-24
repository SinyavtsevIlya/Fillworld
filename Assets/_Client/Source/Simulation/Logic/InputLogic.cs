using System.Linq;
using Zenject;
using UniRx;
using CleanRx;
using UnityEngine;

namespace FillWorld
{
    public class InputLogic : IInitializable
    {
        [Inject] Position2D _screenPosition;
        [Inject] CursorDown _cursorDown;
        [Inject] CompositeDisposable _lifecycle;

        public void Initialize()
        {
            Observable.EveryUpdate()
                .Select(_ => Input.GetMouseButton(0))
                .ToReactiveProperty()
                .Subscribe(value => _cursorDown.Value = value)
                .AddTo(_lifecycle);

            Observable.EveryUpdate()
                .Select(_ => Input.mousePosition)
                .Subscribe(mousePos => _screenPosition.Value = mousePos)
                .AddTo(_lifecycle);
        }
    }
}