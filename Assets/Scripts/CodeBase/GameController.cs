using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using TMPro;
using CodeBase.Leveling;
using CodeBase.Spawning;
using Facebook.Unity;
using CodeBase.Sound;

namespace CodeBase
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private List<ParticleSystem> _winParticles;

        public int numberOfActive;
        public int Collectables;
        public int PairSpawnCount;
        public int levelTime;
        private int _currentTime;
        private bool _isWin;
        private WaitForSeconds _waitForSecondYieldInstruction = new(1);

        private SceneHandler _sceneHandler;
        private LevelHandler _levelHandler;
        private ItemSpawner _spawner;
        private SoundPlayer _soundPlayer;

        public bool IsNeedToSpawn = false;
        public bool isTimerOn = true;

        private void Awake()
        {
            _sceneHandler = FindObjectOfType<SceneHandler>();
            _levelHandler = FindObjectOfType<LevelHandler>();
            _spawner = FindObjectOfType<ItemSpawner>();
            _soundPlayer = FindObjectOfType<SoundPlayer>();

            Application.targetFrameRate = 60;
            
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else {
                //Handle FB.Init
                FB.Init( () => {
                    FB.ActivateApp();
                });
            }
        }

        // Unity will call OnApplicationPause(false) when an app is resumed
        // from the background
        void OnApplicationPause (bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pauseStatus) {
                //app resume
                if (FB.IsInitialized) {
                    FB.ActivateApp();
                } else {
                    //Handle FB.Init
                    FB.Init( () => {
                        FB.ActivateApp();
                    });
                }
            }
        }

        private void Start() => _isWin = false;

        private void Update()
        {
            var min = Mathf.Floor(_currentTime / 60);
            var sec = _currentTime - min * 60;
            _timeText.text = $"{min:00}:{sec:00}";

            if (!_isWin && PairSpawnCount > 0 && PairSpawnCount == Collectables)
            {
                _soundPlayer.Play(Sounds.Win);
                _sceneHandler.ShowWin();
                _levelHandler.IncreaseLevelNumber();
                _isWin = true;
                StopAllCoroutines();
                // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Time : {_currentTime}", _levelHandler.LevelNumber);
                fireEvent(GAProgressionStatus.Complete, "Level Complete", _currentTime);
            }
        }
        
        public void StartLevel()
        {
            _levelHandler.SetLevel(_levelHandler.LevelNumber);
            _isWin = false;
            Collectables = 0;
            numberOfActive = 0;
            StopCoroutine(Countdown());

            if (!IsNeedToSpawn)
                _spawner.Spawn(PairSpawnCount);

            SetTime(!isTimerOn ? 99999 : levelTime);

            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Time : {_currentTime}", _levelHandler.LevelNumber);
            fireEvent(GAProgressionStatus.Start, "Level Start", _currentTime);

            _winParticles.ForEach(particle => particle.Stop());
            
            StartCoroutine(Countdown());
        }

        public void Collect()
        {
            Collectables++;

            if (Collectables == PairSpawnCount)
                _winParticles.ForEach(particle => particle.Play());
        }

        private void SetTime(int time) => _currentTime = time;
        
        private IEnumerator Countdown()
        {
            while (_currentTime > 0)
            {
                yield return _waitForSecondYieldInstruction;
                _currentTime--;
            }

            _sceneHandler.ShowNoTime();
            // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Time : {_currentTime}", _levelHandler.LevelNumber);
            fireEvent(GAProgressionStatus.Fail, "Level Failed", _currentTime);
            
            yield break;
        }

        private void fireEvent(GAProgressionStatus GAname, string FBname, int _currentTime) {
            GameAnalytics.NewProgressionEvent(GAname, $"Time : {_currentTime}", _levelHandler.LevelNumber);

            var tutParams = new Dictionary<string, object>();
            // tutParams[AppEventParameterName.Time] = _currentTime.ToString();
            tutParams[AppEventParameterName.Level] = _levelHandler.LevelNumber;
            tutParams[AppEventParameterName.Description] = FBname;

            FB.LogAppEvent(
                AppEventName.AchievedLevel,
                parameters: tutParams
            );
        }
    }
}