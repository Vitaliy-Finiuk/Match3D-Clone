using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace CodeBase.ScoreCounting
{
    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField, Range(3, 10)] private int _maxMultiply;
        [SerializeField, Range(0, 100)] private float _multiplyTimeLimit;
        [SerializeField] private string[] _positiveFeedbacks;
        [SerializeField] private string[] _negativeFeedbacks;
        [SerializeField] private Image _tweeningScoreImagePrefab;
        [SerializeField, Range(0, 5)] private float _tweenDuration;
        [SerializeField, Range(0, 5)] private float _feedbackDuration;
        [SerializeField, Range(0, 5)] private float _punchForce;
        [SerializeField] private Color _positiveFeddbackColor;
        [SerializeField] private Color _negativeFeddbackColor;

        private int _score;
        private int _multiply;
        private int _minMultiply = 1;
        private bool _isMultiplyTimer;
        private float _multiplyTime;
        private float _tweenDurationDelay = 0.1f;
        private ScorePanel _scorePanel;
        private Slider _timerView;
        private TMP_Text _scoreView;
        private TMP_Text _multiplyView;
        private TMP_Text _scoreFeedback;
        private Image _icon;
        private ObjectPool<Image> _tweeningImagePool;
        private Vector2 _matchPosition;

        private Camera _camera;
        private readonly float _punchDuration = 0.25f;
        private readonly int _punchVibrato = 5;

        public int Score
        {
            get => _score;

            private set => _score = value;
        }

        public int Multiply
        {
            get => _multiply;

            private set
            {
                _multiply = Mathf.Clamp(value, _minMultiply, _maxMultiply);
            }
        }

        private void Awake()
        {
            _scorePanel = FindObjectOfType<ScorePanel>();
            _timerView = _scorePanel.GetComponentInChildren<Slider>();
            _scoreView = _scorePanel.GetComponentInChildren<Score>().GetComponent<TMP_Text>();
            _scoreFeedback = _scorePanel.GetComponentInChildren<ScoreFeedback>().GetComponent<TMP_Text>();
            _multiplyView = _scorePanel.GetComponentInChildren<ScoreMultiply>().GetComponent<TMP_Text>();
            _icon = _scorePanel.GetComponentInChildren<ScoreIcon>().GetComponent<Image>();
            _tweeningImagePool = new(CreateTweeningImage, GetTweeningImage, ReturnTweeningImage, DestroyTweeningImage, false);
            _scoreFeedback.gameObject.SetActive(false);
            _camera = Camera.main;
        }

        private void Start() => ResetMultiplier();

        public void Add(Vector3 matchPosition)
        {
            float tweenDuration = _tweenDuration;
            _matchPosition = RectTransformUtility.WorldToScreenPoint(_camera, matchPosition);

            if (_isMultiplyTimer)
            {
                Multiply++;
                _multiplyView.text = $"{Multiply}X";
                _multiplyView.rectTransform.DOPunchScale(Vector3.one * _punchForce, _punchDuration, _punchVibrato);
                _multiplyTime = _multiplyTimeLimit;
            }
            else
                StartCoroutine(MultiplyTimer());

            for (int i = 0; i < Multiply; i++)
            {
                tweenDuration = _tweenDuration + i * _tweenDurationDelay;
                Image tweeningImage = _tweeningImagePool.Get();
                DOTween.Sequence()
                    .Append(tweeningImage.rectTransform.DOPunchScale(Vector3.one * _punchForce, _punchDuration, _punchVibrato))
                    .Append(tweeningImage.rectTransform.DOMove(_icon.rectTransform.position, tweenDuration).SetEase(Ease.InBack))
                    .Append(tweeningImage.rectTransform.DOPunchScale(Vector3.one * _punchForce, _punchDuration, _punchVibrato))
                    .OnComplete(() =>
                    {
                        Score++;
                        _scoreView.text = Score.ToString();
                        _tweeningImagePool.Release(tweeningImage);
                    });
            }
            ShowFeedback(true);
        }

        public void ResetMultiplier()
        {
            Multiply = _minMultiply;
            _multiplyTime = _timerView.value = 0;
            _multiplyView.text = $"{Multiply}X";
        }

        private IEnumerator MultiplyTimer()
        {
            _isMultiplyTimer = true;
            _multiplyTime = _multiplyTimeLimit;

            while (_multiplyTime > 0)
            {
                yield return null;
                _multiplyTime -= Time.deltaTime;
                _timerView.value = _multiplyTime / _multiplyTimeLimit;
            }

            ResetMultiplier();
            _isMultiplyTimer = false;
            yield break;
        }

        public void ShowFeedback(bool isPositive)
        {
            _scoreFeedback.gameObject.SetActive(true);
            _scoreFeedback.color = isPositive ? _positiveFeddbackColor : _negativeFeddbackColor;
            _scoreFeedback.text = isPositive ? _positiveFeedbacks[Multiply - 1] : _negativeFeedbacks[Random.Range(0, _negativeFeedbacks.Length)];

            _scoreFeedback.rectTransform
                .DOPunchScale(Vector3.one * _punchForce, _feedbackDuration, _punchVibrato, 0)
                .OnComplete(() => _scoreFeedback.gameObject.SetActive(false));
        }

        private Image CreateTweeningImage()
        {
            Image createdTweeningImage = Instantiate(_tweeningScoreImagePrefab, _scorePanel.transform);
            createdTweeningImage.gameObject.SetActive(false);
            return createdTweeningImage;
        }

        private void GetTweeningImage(Image tweeningImage)
        {
            tweeningImage.rectTransform.position = _matchPosition;
            tweeningImage.gameObject.SetActive(true);
        }

        private void ReturnTweeningImage(Image tweeningImage) => tweeningImage.gameObject.SetActive(false);

        private void DestroyTweeningImage(Image tweeningImage) => Destroy(tweeningImage);
    }
}
