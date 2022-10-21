using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _winSound;
        [SerializeField] private AudioSource _matchingSound;
        [SerializeField] private AudioSource _selectionSound;
        [SerializeField] private AudioSource _positiveSelectionSound;
        [SerializeField] private AudioSource _negativeSelectionSound;

        private Dictionary<Sounds, AudioSource> _sounds;

        private void Awake()
        {
            _sounds = new()
            {
                { Sounds.Win, _winSound },
                { Sounds.Disappear, _matchingSound },
                { Sounds.Selection, _selectionSound },
                { Sounds.PositiveSelection, _positiveSelectionSound },
                { Sounds.NegativeSelection, _negativeSelectionSound }
            };
        }

        public void Play(Sounds sounds) => _sounds[sounds]?.Play();
    }
}
