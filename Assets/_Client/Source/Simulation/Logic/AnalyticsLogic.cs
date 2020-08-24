using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class AnalyticsLogic : IInitializable
    {
        [Inject] readonly LevelIndex _levelIndex;
        [Inject] readonly CompositeDisposable _lifecycle;

        Group _actors;
        [Inject]
        void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup().With<Name, Hinted>();
        }

        public void Initialize()
        {
            _levelIndex
                .SkipLatestValueOnSubscribe()
                .Subscribe(id => TrackEvent($"Level_{id}_Complete"))
                .AddTo(_lifecycle);

            _actors.ObserveAdd
                .GroupBy(a => a.Get<Name>().Value)
                .Subscribe(actor => TrackEvent($"hint_used_{actor.Key}"))
                .AddTo(_lifecycle);
        }

        void TrackEvent(string name)
        {
#if UNITY_EDITOR
            Debug.Log($"Track event {name}");
#else
#if UNITY_APPMETRICA
		    AppMetrica.Instance.ReportEvent(name);  
#endif
#endif
        }

        void TrackEvent(string name, Dictionary<string, object> parameters)
        {
#if UNITY_EDITOR
            Debug.Log($"Track event {name} with {parameters.ToVerboseString()}");
#else
#if UNITY_APPMETRICA
		    AppMetrica.Instance.ReportEvent(name, parameters);  
#endif
#endif
        }

        static Dictionary<string, object> EventParams(string value, object obj)
        {
            var result = new Dictionary<string, object>();
            result.Add(value, obj);
            return result;
        }
    }

    static class AnalyticExtensions
    {
        public static Dictionary<string, string> Cast(this Dictionary<string, object> dict)
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in dict)
            {
                result.Add(kvp.Key, kvp.Value.ToString());
            }
            return result;
        }

        public static Dictionary<string, object> And(this Dictionary<string, object> dict, string value, object obj)
        {
            dict.Add(value, obj);
            return dict;
        }

        public static string ToVerboseString(this Dictionary<string, object> dict)
        {
            string result = string.Empty;
            foreach (var kvp in dict)
            {
                var line = " { " + kvp.Key + " : " + kvp.Value + " } ";
                result += System.Environment.NewLine + line;
            }
            return result;
        }
    }
}