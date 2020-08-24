using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class CreateLevelLogic : IInitializable
    {
        [Inject] readonly RoadmapSettings _settings;
        [Inject] readonly LevelIndex _levelIndex;
        [Inject] readonly SceneAsset _scene;
        [Inject] readonly CharPattern _charPattern;
        [Inject] readonly CharSequence _combo;
        [Inject] readonly ActorFactory _actorFactory;
        [Inject] readonly ExtraActorFactory _extraActorFactory;
        [Inject] readonly LetterFactory _letterFactory;
        [Inject] readonly Entity _level;
        [Inject] readonly CleanRx.Context _context;

        List<Entity> _destroyPool = new List<Entity>();
        Group _actors;
        Group _extraActors;
        [Inject]
        void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup()
                .With<Name>()
                .Without<IsExtra>();

            _extraActors = context.GetGroup()
                .With<Name, IsExtra>();
        }

        public void Initialize()
        {
            _levelIndex
                .Subscribe(index => Observable.FromCoroutine(t => LoadLevelRoutine(index)).Subscribe());
        }

        IEnumerator LoadLevelRoutine(int index)
        {
            ClearLevel();

            var levelAsset = _settings.GetLevel(index);
            var parameters = new LoadSceneParameters(LoadSceneMode.Additive);
            _scene.Value = SceneManager.LoadScene(levelAsset, parameters);

            yield return null;

            var levelGameObject = _scene.Value.GetRootGameObjects().First(x => x.GetComponent<LevelSettings>() != null);
            var levelSettings = levelGameObject.GetComponent<LevelSettings>();

            foreach (var letter in levelSettings.Pattern)
            {
                _charPattern.Add(_letterFactory.Create(letter));
            }

            if (levelSettings.HasExtraWords)
            {
                foreach (var extraWord in levelSettings.ExtraWords)
                {
                    var extraActor = _extraActorFactory.Create(extraWord);
                }
            }

            var actorsSettings = levelGameObject.GetComponentsInChildren<ActorSettings>();
            foreach (var actorSettings in actorsSettings)
            {
                _actorFactory.Create(actorSettings);
            }

            _actors.GroupBy(a => a.Get<Name>().Value).ToList().ForEach(g => g.ToList().ForEach(a => a.Get<OrderIndex>().Value = g.ToList().IndexOf(a)));

            var env = levelSettings.Environment;

            RenderSettings.ambientSkyColor = env.SkyColor;
            RenderSettings.ambientEquatorColor = env.EquatorColor;
            RenderSettings.ambientGroundColor = env.GroundColor;
            RenderSettings.fog = true;    
            RenderSettings.fogColor = env.Fog.Color;
            RenderSettings.fogMode = env.Fog.Mode;
            Observable.TimerFrame(1).Subscribe(_ => RenderSettings.fogDensity = env.Fog.Dencity);
        }

        void ClearLevel()
        {
            _combo.Clear();

            foreach (var letter in _charPattern)
            {
                _destroyPool.Add(letter);
            }
            _charPattern.Clear();

            
            foreach (var actor in _actors)
            {
                _destroyPool.Add(actor);
            }

            foreach (var actor in _extraActors)
            {
                _destroyPool.Add(actor);
            }

            foreach (var entity in _destroyPool)
            {
                _context.Destroy(entity);
            }
            _destroyPool.Clear();

            if (SceneManager.sceneCount > 1)
            {
                _level.Lifecycle.Clear();
                SceneManager.UnloadSceneAsync(_scene.Value);
            }
        }
    }
}
