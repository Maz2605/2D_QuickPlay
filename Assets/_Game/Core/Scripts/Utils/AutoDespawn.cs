using System.Collections;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using UnityEngine;

namespace _Game.Core.Scripts.Utils
{
    public class AutoDespawn : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1.0f;

        private void OnEnable()
        {
            StartCoroutine(DespawnRoutine());
        }

        private IEnumerator DespawnRoutine()
        {
            yield return new WaitForSeconds(lifetime);
            PoolingManager.Instance.Despawn(this.gameObject);
        }
    }
}
