using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UnityEngine;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class EditorPauseLogic : IInitializable
    {
        public void Initialize()
        {
            var cachedTimeScale = 1f;
            var isPaused = false;

            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => 
                {
                    if (isPaused)
                    {
                        Time.timeScale = cachedTimeScale;
                    }
                    else
                    {
                        cachedTimeScale = Time.timeScale;
                        Time.timeScale = 0f;
                    }
                    isPaused = !isPaused;
                });
        }
    }
}
