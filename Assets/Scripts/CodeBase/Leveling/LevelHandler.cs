using System;
using System.Linq;
using UnityEngine;
using CodeBase.ScoreCounting;

namespace CodeBase.Leveling
{
    public class LevelHandler : MonoBehaviour
    {
        private GameController _gameController;
        private ScoreCounter _scoreCounter;
        
        public int LevelNumber;
        public LevelSet[] LevelSet;

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();
            _scoreCounter = FindObjectOfType<ScoreCounter>();
        }

        public void SetLevelNumber(int number) => LevelNumber = number;

        public void IncreaseLevelNumber()
        {
            if (LevelNumber < LevelSet.Count() - 1)
                LevelNumber++;
        }

        public void SetLevel(int number)
        {
            _scoreCounter.ResetMultiplier();
            _gameController.PairSpawnCount = LevelSet[number].PairNumber;
            _gameController.isTimerOn = LevelSet[number].ActiveTimer;
            _gameController.levelTime = LevelSet[number].LevelTime;
            _gameController.IsNeedToSpawn = false;
        }

        public void SetLevelAuto()
        {
            _gameController.PairSpawnCount = LevelSet[LevelNumber].PairNumber;
            _gameController.isTimerOn = LevelSet[LevelNumber].ActiveTimer;
            _gameController.levelTime = LevelSet[LevelNumber].LevelTime;
        }
    }

    [Serializable]
    public class LevelSet
    {
        public bool ActiveTimer = true;
        public int LevelNumber = 0;
        public int LevelTime = 0;
        public int PairNumber = 0;
    }
}