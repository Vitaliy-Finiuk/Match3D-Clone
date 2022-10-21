using UnityEngine;
using GameAnalyticsSDK;

namespace Analytic
{
    public class AnalyticsInitializer : MonoBehaviour, IGameAnalyticsATTListener
    {
        private void Start() => GameAnalytics.Initialize();

        public void GameAnalyticsATTListenerAuthorized() => GameAnalytics.Initialize();

        public void GameAnalyticsATTListenerDenied() => GameAnalytics.Initialize();

        public void GameAnalyticsATTListenerNotDetermined() => GameAnalytics.Initialize();

        public void GameAnalyticsATTListenerRestricted() => GameAnalytics.Initialize();
    }
}