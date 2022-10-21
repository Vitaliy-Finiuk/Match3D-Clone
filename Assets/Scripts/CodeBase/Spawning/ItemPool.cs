using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CodeBase.Matching;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

namespace CodeBase.Spawning
{
    public class ItemPool
    {
        private List<Item> _items = new();

        public ItemPool(IEnumerable<ItemData> itemsData, Transform container)
        {
            for (int i = 0; i < itemsData.Count(); i++)
            {
                ItemData data = itemsData.ElementAt(i);
                Item item = Object.Instantiate(Resources.Load("Prefabs/Item"), container).GetComponentInChildren<Item>();
                item.name = data.name;
                item.Set(data);
                item.gameObject.SetActive(false);
                _items.Add(item);
            }
        }

        public Item Get(Vector3 spawnPosition)
        {
            IEnumerable<Item> availableItems = _items.Where(item => item.gameObject.activeInHierarchy == false);
            Item item = availableItems.ElementAt(Random.Range(0, availableItems.Count()));
            item.transform.SetPositionAndRotation(spawnPosition, Random.rotation);
            item.gameObject.SetActive(true);
            return item;
        }

        public Item Get(int id, Vector3 spawnPosition)
        {
            Item item = _items.Find(item => item.Id == id);

            if (item == null || item.gameObject.activeInHierarchy)
                throw new Exception("Failed to get an item by this id");

            item.transform.SetPositionAndRotation(spawnPosition, Random.rotation);
            item.gameObject.SetActive(true);
            return item;
        }
    }
}
