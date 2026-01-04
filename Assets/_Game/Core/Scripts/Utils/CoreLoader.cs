using System;
using _Game.Core.Scripts.Manager;
using UnityEngine;

namespace _Game.Core.Scripts.Utils
{
    public class CoreLoader : MonoBehaviour
    {
        [SerializeField] private GameObject corePrefab;

        private void Awake()
        {
            if (FindObjectOfType<AudioManager>() == null)
                Instantiate(corePrefab);
            
            Destroy(gameObject);
        }
    }
}
