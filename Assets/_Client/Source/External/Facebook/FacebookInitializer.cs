using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_FACEBOOK
using Facebook.Unity;
#endif

public class FacebookInitializer : MonoBehaviour
{
#if UNITY_FACEBOOK
    private static FacebookInitializer instance;
    private bool initCalled;

    private void Awake()
    {
        Debug.Log("FB Awake");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (FB.IsInitialized == false && initCalled == false)
        {
            FB.Init();
            initCalled = true;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            if (FB.IsInitialized == true)
            {
                FB.ActivateApp();
            }
            else
            {
                if (initCalled == false)
                {
                    FB.Init();
                    initCalled = true;
                }
                StartCoroutine(ActivateEvent());
            }
        }
    }

    private IEnumerator ActivateEvent()
    {
        yield return new WaitUntil(() => FB.IsInitialized == true);
        Debug.Log("Facebook Activate Event Logged");
        FB.ActivateApp();
    }
#endif
}
