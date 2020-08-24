using UnityEngine;
using Zenject;
using CleanRx;
using DG.Tweening;
using UniRx;

namespace FillWorld
{   
    public class ActorInstaller : Installer<ActorInstaller>
    {
        [Inject] CleanRx.Context _context;
        [Inject] ActorSettings _actorSettings;
        [Inject] ActorViewSettings _actorViewSettings;

        public override void InstallBindings()
        {
            var entity = _context.CreateEntity();
            entity.Add<Name>().Value = _actorSettings.Name;
            entity.Add<View>().Value = _actorSettings.gameObject;
            entity.Add<Position>().Value = _actorSettings.transform.position;
            entity.Add<OrderIndex>();
            Container.BindInstance(entity).AsSingle();

            var lifecycle = new CompositeDisposable();

            entity.OnDestroy += () => { lifecycle.Clear(); };

            Container.BindInstance(lifecycle).AsSingle();

            Container.BindInterfacesTo<ActorPresenter>()
                .FromNewComponentOn(_actorSettings.gameObject)
                .AsSingle()
                .WithArguments(_actorViewSettings);

            Container.BindSubKernel();
        }
    } 
}