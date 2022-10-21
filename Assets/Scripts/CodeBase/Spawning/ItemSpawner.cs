using System.Collections.Generic;
using UnityEngine;
using CodeBase.Matching;
using Random = UnityEngine.Random;

namespace CodeBase.Spawning
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _itemsData;
        [SerializeField] private float _minX, _maxX, _minY, _maxY, _minZ, _maxZ;

        private List<Item> _allItems = new();
        private ItemPool _items;

        private Matcher _matchingHandler;

        private void Awake() => _matchingHandler = FindObjectOfType<Matcher>();

        private void Start() => _items = new(_itemsData, transform);

        public void Spawn(int pairsCount)
        {
            Item item;
            _matchingHandler.ResetState();

            for (var i = 0; i < pairsCount; i++)
            {
                _allItems.Add(item = _items.Get(GetRandomPos()));
                _allItems.Add(_items.Get(item.Pair.Id, GetRandomPos()));
            }
        }

        private Vector3 GetRandomPos() => new(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), Random.Range(_minZ, _maxZ));
    }
}