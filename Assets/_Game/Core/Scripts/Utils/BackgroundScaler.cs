using UnityEngine;

namespace _Game.Core.Scripts.Utils
{
    public class BackgroundScaler : MonoBehaviour
    {
        void Start()
        {
            var sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr != null || Camera.main == null) return;
            float cameraHeight = Camera.main.orthographicSize * 2;
            Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
            Vector2 spriteSize = sr.sprite.bounds.size;

            Vector2 scale = transform.localScale;
            if (cameraSize.x >= cameraSize.y)
            {
                // Landscape
                scale *= cameraSize.x / spriteSize.x;
            }
            else
            {
                // Portrait
                scale *= cameraSize.y / spriteSize.y;
            }

            transform.localScale = scale;
        }
    }
}