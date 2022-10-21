using System.Collections;
using DG.Tweening;
using UnityEngine;
using CodeBase.ScoreCounting;
using CodeBase.Sound;

namespace CodeBase.Matching
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float _motionDuration = 0.1f;
        [SerializeField, Range(0, 100)] private float _rotationSpeed = 15;
        [SerializeField] private AudioClip _collisionSound;

        private Tweener _dragingTweener;

        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private Rigidbody _rigidbody;
        private Camera _camera;
        private Matcher _matchingHandler;
        private AudioSource _audioSource;
        private Canvas _canvas;
        private ScoreCounter _scoreCounter;
        private SoundPlayer _soundPlayer;

        public int Id { get; private set; }
        public ItemData Pair { get; private set; }
        public Outline Outline { get; private set; }
        public MatchingSide MatchingSide { get; private set; }
        public Animation _animation { get; private set; }

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _collider = GetComponent<MeshCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _animation = GetComponent<Animation>();
            Outline = GetComponent<Outline>();
            _camera = Camera.main;
            _matchingHandler = Matcher.Instance;
            _audioSource = GetComponent<AudioSource>();
            _canvas = FindObjectOfType<Canvas>();
            _scoreCounter = FindObjectOfType<ScoreCounter>();
            _soundPlayer = FindObjectOfType<SoundPlayer>();
        }

        public void Set(ItemData data)
        {
            Id = data.Id;
            Pair = data.Pair;
            MatchingSide = data.MatchingSide;
            _meshFilter.sharedMesh = data.Mesh;
            _collider.sharedMesh = data.Mesh;
            Outline.LoadSmoothNormals();

            if (data.MatchingAnimation == null)
            {
                switch (MatchingSide)
                {
                    case MatchingSide.Left: _animation.clip = Resources.Load<AnimationClip>("Animations/DefaultLeft"); break;

                    case MatchingSide.Right: _animation.clip = Resources.Load<AnimationClip>("Animations/DefaultRight"); break;
                }
            }
            else
                _animation.clip = data.MatchingAnimation;
        }

        public void Hide()
        {
            Outline.enabled = false;
            _rigidbody.isKinematic = false;
            gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            if (_matchingHandler.State != MatchingState.Full) 
            {
                _dragingTweener = transform.DOMove(GetOffsetedDragingPosition(), _motionDuration).SetAutoKill(false);
                Select();
                _rigidbody.isKinematic = true;
                Outline.enabled = true;
            }
        }

        private void OnMouseDrag()
        {
            if (_matchingHandler.State != MatchingState.Full) 
            {
                _dragingTweener?.ChangeEndValue(GetOffsetedDragingPosition(), true).Restart();
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, -Input.GetAxis("Mouse X")) * _rotationSpeed / _canvas.transform.localScale.x, Space.World);
            }
        }

        private void OnMouseUp() => _dragingTweener?.Kill();

        private void Select()
        {
            switch (_matchingHandler.State)
            {
                case MatchingState.Empty:
                    _soundPlayer.Play(Sounds.Selection);
                    _rigidbody.isKinematic = true;
                    _matchingHandler.State = MatchingState.Finding;
                    _matchingHandler.FirstItem = this;
                    break;

                case MatchingState.Finding:
                    if (_matchingHandler.IsMatch(Id))
                    {
                        _soundPlayer.Play(Sounds.PositiveSelection);
                        _soundPlayer.Play(Sounds.Disappear);
                        _rigidbody.isKinematic = true;
                        _matchingHandler.State = MatchingState.Full;
                        _matchingHandler.SecondItem = this;
                        _matchingHandler.Match();
                    }
                    else
                    {
                        if (_matchingHandler.FirstItem != this)
                        {
                            _soundPlayer.Play(Sounds.NegativeSelection);
                            _scoreCounter.ShowFeedback(false);
                        }

                        _rigidbody.isKinematic = true;
                        _matchingHandler.FirstItem._rigidbody.isKinematic = false;
                        _matchingHandler.FirstItem.Outline.enabled = false;
                        _matchingHandler.FirstItem = this;

                    }
                    break;

                case MatchingState.Full:
                    break;
            }
        }

        public IEnumerator Matching(Vector3 matchingPosition)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Insert(0, transform.DORotate(new(-_camera.transform.eulerAngles.x, 180), 0.5f));

            sequence
                .Insert(0, transform
                .DOMove(MatchingSide == MatchingSide.Left ? Vector3.left : Vector3.right, 0.5f)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => _animation?.Play()));

            yield return new WaitForSeconds(_animation.clip.length);
            Hide();
            yield break;
        }

        private Vector3 GetOffsetedDragingPosition()
        {
            float dragingHeight = 0;
            Vector3 dragingPositionOnScrean = new(Input.mousePosition.x, Input.mousePosition.y, _camera.WorldToScreenPoint(transform.position).z);
            Vector3 dragingPosition = _camera.ScreenToWorldPoint(dragingPositionOnScrean);
            return new(dragingPosition.x, dragingHeight, dragingPosition.z);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out Ground ground))
            {
                _audioSource.clip = _collisionSound;
                _audioSource?.Play();
            }
        }
    }
}