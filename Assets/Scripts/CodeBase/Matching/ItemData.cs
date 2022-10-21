using UnityEngine;

namespace CodeBase.Matching
{
    [CreateAssetMenu(menuName = "Item", fileName = "New Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private Mesh _mesh;
        [SerializeField] private ItemData _pair;
        [SerializeField] private AnimationClip _matchingAnimation;
        [SerializeField] private MatchingSide _matchingSide;

        public int Id => _id;
        public Mesh Mesh => _mesh;
        public ItemData Pair => _pair;
        public AnimationClip MatchingAnimation => _matchingAnimation;
        public MatchingSide MatchingSide => _matchingSide;
    }

    public enum MatchingSide
    {
        Left,
        Right,
    }
}
