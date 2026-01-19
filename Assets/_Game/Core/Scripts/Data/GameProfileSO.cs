using UnityEngine;

namespace _Game.Core.Scripts.Data
{
    [CreateAssetMenu(fileName = "New Game Profile", menuName = "Game Profile")]
    public class GameProfileSO : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite uiSprite;
        public string sceneName;
    }
}