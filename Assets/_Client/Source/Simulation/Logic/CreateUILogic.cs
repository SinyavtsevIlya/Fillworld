using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UnityEngine;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class CreateUILogic : IInitializable
    {
        [Inject] readonly UIService _ui;

        public void Initialize()
        {
            _ui.Initialize<CoreWindow>();
        }
    }
}
