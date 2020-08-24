using System;
using UnityEngine;
using Zenject;
using UniRx;
using System.Reflection;
using CleanRx;
using System.Linq;
using System.Collections.Generic;

namespace FillWorld
{
    public class CoreInstaller : MonoInstaller
    {
        [Inject] CoreSettings _coreSettings;

        Entities _entities;
        CleanRx.Context _coreContext;

        class Entities
        {
            public Entity Level;
            public Entity Player;
            public Entity Input;

            public Entity[] Values;

            public Entities(CleanRx.Context context)
            {
                Level = context.CreateEntity();
                Player = context.CreateEntity();
                Input = context.CreateEntity();
                Values = new Entity[] { Level, Player, Input };
            }
        }

        public override void InstallBindings()
        {
            InstallCompositeDisposables();
            InstallFactories();
            InstallSettings();
            InstallServices();
            InstallSignals();
            InstallCore();
            InstallLogics();
            InstallUserInterface();
        }

        void InstallServices()
        {
        }

        void InstallUserInterface()
        {
            Container.Bind<UIService>()
                .FromSubContainerResolve()
                .ByInstaller<CoreUIInstaller>()
                .AsSingle()
                .WithArguments(_entities.Level);
        }

        void InstallCore()
        {
            _coreContext = new CleanRx.Context();
            _entities = new Entities(_coreContext);

            _entities.Values.ToList().ForEach(e => e.Composition.ObserveAdd().Subscribe(kvp =>
            {
                var type = kvp.Value.GetType();
                Container.Bind(type).FromInstance(kvp.Value).AsSingle();
            }));

            Container.BindInstance(_coreContext).AsSingle();

            Container.BindInstance(_entities.Level).AsSingle();

            _entities.Level.Add<CharPattern>();
            _entities.Level.Add<SceneAsset>();

            // Install Player
            _entities.Player.Add<LevelIndex>();
            _entities.Player.Add<CharSequence>();
            _entities.Player.Add<ComboResult>();
            _entities.Player.Add<HintRequest>();

            // Install Input
            _entities.Input.Add<Position2D>();
            _entities.Input.Add<CursorDown>();
        }

        void InstallLogics()
        {
            AddLogic<InputLogic>();
            AddLogic<CreateLevelLogic>(_entities.Level);
            AddLogic<CreateUILogic>();
            AddLogic<ValidateComboLogic>();
            AddLogic<CompleteLevelLogic>();
            AddLogic<WordMatchLogic>();
            AddLogic<ResetComboLogic>();
            AddLogic<HintLogic>();
            AddLogic<AnalyticsLogic>();
            AddLogic<EditorPauseLogic>();
        }

        void AddLogic<T>(params object[] arguments)
        {
            Container.BindInterfacesTo<T>().AsSingle().WithArguments(arguments);
        }

        void InstallSignals()
        {
            Container.Bind<LetterSelected>().AsSingle();
            Container.Bind<LevelCompleted>().AsSingle();
        }

        void InstallCompositeDisposables()
        {
            Container.Bind<CompositeDisposable>().AsSingle();
        }

        void InstallFactories()
        {
            Container.BindFactory<char, Entity, LetterFactory>()
                .FromSubContainerResolve()
                .ByInstaller<LetterInstaller>()
                .AsSingle();

            Container.BindFactory<ActorSettings, Entity, ActorFactory>()
                .FromSubContainerResolve()
                .ByInstaller<ActorInstaller>()
                .AsSingle();

            Container.BindFactory<string, Entity, ExtraActorFactory>()
                .FromSubContainerResolve()
                .ByMethod((container, name) => 
                {
                    var e = _coreContext.CreateEntity();
                    container.BindInstance(e).AsSingle();
                    e.Add<Name>().Value = name;
                    e.Add<IsExtra>();
                })
                .AsSingle();
        }

        void InstallSettings()
        {
            var view = _coreSettings.View;
            Container.BindInstance(view.Actor);

            var sim = _coreSettings.Simulation;
            Container.BindInstance(sim.Roadmap);
        }
    }

    
}
