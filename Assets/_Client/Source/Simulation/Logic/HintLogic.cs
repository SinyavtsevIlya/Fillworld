using System.Linq;
using Zenject;
#if UNITY_ADS
using UnityEngine.Advertisements; 
#endif
using UniRx;
using CleanRx;

namespace FillWorld
{
    public class HintLogic : IInitializable 
    {
        [Inject] readonly HintRequest _hintRequest;
        [Inject] readonly CompositeDisposable _lifecycle;

        Group _actors;
        [Inject]
        void GetGroups(CleanRx.Context context)
        {
            _actors = context.GetGroup().With<Name>().Without<IsExtra, Matched, Hinted>();
        }

        public void Initialize()
        {
            InitializeAds();

            _hintRequest.Value
                .Where(_ => _actors.Count > 0)
                .Subscribe(_ =>
                {
#if UNITY_ADS
                    Advertisement.Show("rewardedVideo", new ShowOptions()
                    {
                        resultCallback = (sr) =>
                        {
                            if (sr == ShowResult.Finished) SuggestHint();
                        }
                    }); 
#else
                    SuggestHint();
#endif
                })
                .AddTo(_lifecycle);
        }

        void InitializeAds()
        {
#if UNITY_ADS
            var key = "";

#if UNITY_ANDROID
            key = "3672525";
#endif
            Advertisement.Initialize(key, false); 
#endif
        }

        void SuggestHint()
        {
            var reference = _actors.Last();

            _actors
            .Where(a => a.Get<Name>().Value == reference.Get<Name>().Value).ToList()
            .ForEach(a => a.Add<Hinted>());
        }
    }
}
