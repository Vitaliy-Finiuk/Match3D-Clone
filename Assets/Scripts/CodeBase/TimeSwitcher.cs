using UnityEngine;

namespace CodeBase
{
    public class TimeSwitcher : MonoBehaviour
    {
        public GameController controller;
        public GameObject timerText;

        private void Start()
        {
            var gameGO = GameObject.FindGameObjectWithTag("GameController");
            controller = gameGO.GetComponent<GameController>();
        }

        private void Update() => 
            timerText.SetActive(controller.isTimerOn);
    }
}