using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Sound
{
    public class SoundButton : MonoBehaviour
    {
        private bool _isOn = true;
        private Image _image;
        private Button _button;
        private Sprite _onSprite;
        private Sprite _offSprite;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _onSprite = Resources.Load<Sprite>("Game GUI Vol1/LazyDay/PNG/SoundON");
            _offSprite = Resources.Load<Sprite>("Game GUI Vol1/LazyDay/PNG/SoundOFF");
        }

        private void OnEnable() => _button.onClick.AddListener(Switch);

        public void Switch()
        {
            _isOn = !_isOn;
            _image.sprite = _isOn ? _onSprite : _offSprite;
            AudioListener.volume = _isOn ? 1 : 0;
        }

        private void OnDisable() => _button.onClick.RemoveListener(Switch);
    }
}