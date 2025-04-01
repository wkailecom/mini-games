using Firebase;
using Firebase.Analytics;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sdk
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        const string FCM_TOKEN = "FCMToken";

        static FirebaseApp firebaseApp;

        string mFCMToken;
        public string FCMToken
        {
            get { return string.IsNullOrEmpty(mFCMToken) ? mFCMToken = PlayerPrefs.GetString(FCM_TOKEN) : mFCMToken; }
            private set { PlayerPrefs.SetString(FCM_TOKEN, mFCMToken = value); }
        }

        bool isAddFCMEvent;

        public void Init()
        {
            TaskManager.Instance.StartCoroutine(FirebaseInit());
        }

        public void UnInit()
        {
            UnInitMessaging();
        }

        IEnumerator FirebaseInit()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log("Firebase Could ok:" + dependencyStatus);
                    InitializeFirebase();
                }
                else
                {
                    Debug.Log("FirebaseManager.CheckDependency: Could not resolve all Firebase dependencies:" + dependencyStatus);
                }
            });
            yield return null;
        }

        void InitializeFirebase()
        {
            firebaseApp = FirebaseApp.DefaultInstance;

            InitMessaging();
        }


        #region Messaging
        public void InitMessaging()
        {
#if UNITY_ANDROID
            Debug.Log("InitMessaging android");
            isAddFCMEvent = true;
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
#elif UNITY_IOS
        //var hasLoading = NotificationManager.Instance.IsAuthorization;
        //Debug.Log("InitMessaging ios : " + hasLoading);
        //if (hasLoading)
        //{
        //    isAddFCMEvent = true;
        //    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        //    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        //}
#endif
        }

        public void UnInitMessaging()
        {
            if (firebaseApp != null)
            {
                if (isAddFCMEvent)
                {
                    Debug.Log("FirebaseManager.UnInitMessaging: Remove FCM event");
                    Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
                    Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
                    isAddFCMEvent = false;
                }
                firebaseApp = null;
            }
        }

        void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            if (token?.Token != null)
            {
                FCMToken = token.Token;
                Debug.Log("Received Registration Token: " + token.Token);
            }
        }

        void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            //Debug.Log("Received a new message from1: " + e);
            //Debug.Log("Received a new message from: " + e.Message.From);
            //Debug.Log("Messaging ID: " + e.Message.MessageId);
            //Debug.Log("Messaging Badge: " + e.Message.Notification.Badge);
            //Debug.Log("Messaging Title: " + e.Message.Notification.Title);
            //Debug.Log("Messaging Body: " + e.Message.Notification.Body);
        }

        #endregion


        public static void LogAppEvent(string eventName, params Parameter[] parameters)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);
        }

        public static void LogAppEvent(string eventName)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
    }
}