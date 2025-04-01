using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using Facebook.Unity;
using LLFramework;

namespace Game.Sdk
{
    public class FacebookManager : Singleton<FacebookManager>
    {
        public void Init()
        {
            if (FB.IsInitialized)
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
            else
            {
                // Initialize the Facebook SDK
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        // Signal an app activation App Event
                        FB.ActivateApp();
                        // Continue with Facebook SDK
                    }
                    else
                    {
                        Debug.LogError("Failed to Initialize the Facebook SDK");
                    }
                });
            }
        }
    }
}