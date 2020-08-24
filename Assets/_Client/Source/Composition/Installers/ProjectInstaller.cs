using System;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] CoreSettings _core;

        public override void InstallBindings()
        {
            Container.BindInstance(_core).AsSingle();
        }
    }
}
