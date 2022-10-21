using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Leveling
{
    public class LevelTextBinder : MonoBehaviour
    {
        [SerializeField] private GameObject _controller;
        [SerializeField] private Text _textComp;

        private void Start()
        {
            _controller = GameObject.FindGameObjectWithTag("Master");
            _textComp = gameObject.GetComponent<Text>();
        }

        private void Update()
        {
            var rep = _controller.GetComponent<LevelHandler>().LevelNumber;
            Debug.Log("String to change: " + rep.ToString());
            string output = _textComp.text.Replace("{NUM}", rep.ToString());
            _textComp.text = output;
            Debug.Log(_textComp.text);
        }
    }
}