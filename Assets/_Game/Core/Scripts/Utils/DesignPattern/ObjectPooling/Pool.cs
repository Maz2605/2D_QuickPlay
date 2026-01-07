using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling
{
    public class Pool
    {
        private readonly Queue<GameObject> _pools;
        public readonly HashSet<int> IDObject;
        private readonly GameObject _prefabObject;
        private int _id = 0;
    
        public Pool(GameObject prefab)
        {
            _prefabObject = prefab;
            _pools = new Queue<GameObject>();
            IDObject = new HashSet<int>();
        }
    
        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            while (true)
            {
                GameObject newObject;
                if (_pools.Count == 0)
                {
                    newObject = Object.Instantiate(_prefabObject, position, rotation, parent);
                    _id++;
                    IDObject.Add(newObject.GetInstanceID());
                    newObject.name = _prefabObject.name + "_" + _id;
                    return newObject;
                }

                newObject = _pools.Dequeue();
                if (newObject == null) continue;

                newObject.transform.SetPositionAndRotation(position, rotation);
                newObject.transform.SetParent(parent);
                newObject.SetActive(true);
                return newObject;
            }
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            return Spawn(position, rotation, parent).GetComponent<T>();
        }

        public void Despawn(GameObject gameObject)
        {
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(false);
            _pools.Enqueue(gameObject);
        }
    }
}
