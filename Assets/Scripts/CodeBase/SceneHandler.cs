using UnityEngine;

namespace CodeBase
{
    public class SceneHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _gamePanel;
        [SerializeField] private GameObject _timeOutPanel;
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _failPanel;
    
        private void Start() => ShowMainMenu();

        public void ShowMainMenu()
        {
            _menuPanel.SetActive(true);
            _gamePanel.SetActive(false);
            _timeOutPanel.SetActive(false);
            _winPanel.SetActive(false);
            _failPanel.SetActive(false);
        }

        public void ShowGameplay()
        {
            _menuPanel.SetActive(false);
            _gamePanel.SetActive(true);
            _timeOutPanel.SetActive(false);
            _winPanel.SetActive(false);
            _failPanel.SetActive(false);
        }

        public void ShowNoTime()
        {
            _menuPanel.SetActive(false);
            _gamePanel.SetActive(false);
            _timeOutPanel.SetActive(true);
            _winPanel.SetActive(false);
            _failPanel.SetActive(false);
        }

        public void ShowWin()
        {
            _menuPanel.SetActive(false);
            _gamePanel.SetActive(false);
            _timeOutPanel.SetActive(false);
            _winPanel.SetActive(true);
            _failPanel.SetActive(false);
        }

        public void ShowFail()
        {
            _menuPanel.SetActive(false);
            _gamePanel.SetActive(false);
            _timeOutPanel.SetActive(false);
            _winPanel.SetActive(false);
            _failPanel.SetActive(true);
        }
    }
}
