using System;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class CoreUIInstaller : Installer<CoreUIInstaller>
    {
        [Inject] Entity _level;

        public override void InstallBindings()
        {
            Container.Bind<UIService>().AsSingle();
            Container.Bind<UIRegistry>().AsSingle();
            Container.Bind<UIFactory>().AsSingle().WithArguments("Core");
            Container.InstallWindow<CoreWindow>(_level);
        }
    }
}
