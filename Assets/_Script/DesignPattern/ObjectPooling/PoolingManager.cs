using System.Collections.Generic;
using _Script.DesignPattern.Singleton;
using UnityEngine;

namespace _Script.DesignPattern.ObjectPooling
{
    public class PoolingManager : Singleton<PoolingManager>
    {
        private Dictionary<int, Pool> _listPools;

        protected override void Awake()
        {
            base.Awake();
            if (_listPools == null)
            {
                _listPools = new Dictionary<int, Pool>();
            }
        }

        private void Init(GameObject prefab)
        {
            if (prefab != null && !_listPools.ContainsKey(prefab.GetInstanceID()))
            {
                _listPools[prefab.GetInstanceID()] = new Pool(prefab);
            }
        }

        public GameObject Spawn(GameObject prefab)
        {
            Init(prefab);
            return _listPools[prefab.GetInstanceID()].Spawn(Vector3.zero, Quaternion.identity);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            Init(prefab);
            return _listPools[prefab.GetInstanceID()].Spawn(position, rotation, parent);
        }

        public T Spawn<T>(T prefab) where T : Component
        {
            Init(prefab.gameObject);
            return _listPools[prefab.GetInstanceID()].Spawn<T>(Vector3.zero, Quaternion.identity);
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            Init(prefab.gameObject);
            return _listPools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, rotation, parent);
        }

        public void Despawn(GameObject prefab)
        {
            Pool pool = null;
            foreach (var p in _listPools.Values)
            {
                if (p.IDObject.Contains(prefab.GetInstanceID()))
                {
                    pool = p;
                    break;
                }
            }

            if (pool == null)
            {
                Destroy(prefab);
            }
            else
            {
                pool.Despawn(prefab);
            }
        }

        public void ClearPool()
        {
            _listPools?.Clear();
        }
    }
}
