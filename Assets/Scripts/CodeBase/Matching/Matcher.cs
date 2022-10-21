using System.Collections;
using UnityEngine;
using CodeBase.ScoreCounting;
using CodeBase.Sound;

namespace CodeBase.Matching
{
    public enum MatchingState
    {
        Empty,
        Finding,
        Full    
    }

    public class Matcher : MonoBehaviour
    {
        public static Matcher Instance;

        [SerializeField] private ParticleSystem _particle;
        [SerializeField, Range(0, 100)] private float _selectedItemRotationSpeed;

        private Vector3 _matchingPosition;
        private float _distanceThreshold = 2f;

        private GameController _gameController;
        private ScoreCounter _scoreCounter;
        private Camera _camera;
        private SoundPlayer _soundPlayer;

        public MatchingState State { get; set; } = MatchingState.Empty;
        public Item FirstItem { get; set; }
        public Item SecondItem { get; set; }

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();
            _scoreCounter = FindObjectOfType<ScoreCounter>();
            _camera = Camera.main;
            _soundPlayer = FindObjectOfType<SoundPlayer>();

            _matchingPosition = _camera.ScreenToWorldPoint(new(Screen.width / 2, Screen.height / 2));
            _matchingPosition.y = 0;
            _matchingPosition.z = 1;

            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Update() => FirstItem?.transform.Rotate(Vector3.up, _selectedItemRotationSpeed * Time.deltaTime, Space.World);

        public void Match()
        {
            if(FirstItem == null || SecondItem == null)
            {
                Debug.LogError("Match Error");
                return;
            }

            StartCoroutine(Matching());
            State = MatchingState.Empty;
        }

        public bool IsMatch(int id)
        {
            if (FirstItem == null)
                return false;

            if (FirstItem.Pair.Id == id)
                return true;

            return false;
        }

        public bool IsInRange(Vector3 position)
        {
            var checkerPos = transform.position;
            checkerPos.y = 0;
            position.y = 0;
            var distance = checkerPos - position;
            return distance.magnitude < _distanceThreshold;
        }

        public void ResetState()
        {
            State = MatchingState.Empty;
            FirstItem = null;
            SecondItem = null;
        }

        private IEnumerator Matching()
        {
            StartCoroutine(FirstItem.Matching(_matchingPosition));
            yield return StartCoroutine(SecondItem.Matching(_matchingPosition));
            _scoreCounter.Add(_matchingPosition);
            _particle.transform.position = _matchingPosition;
            _particle.Play();
            // _soundPlayer.Play(Sounds.Disappear);
            _gameController.Collect();
        }
    }
}