using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    private void Awake()
    {
        if (gameObject.scene.name == "UICoreScene")
            return;

        if (HasUICoreMainCamera())
        {
            Destroy(gameObject); 
        }
      
    }

    private bool HasUICoreMainCamera()
    {
        var uiScene = SceneManager.GetSceneByName("UICoreScene");
        if (!uiScene.IsValid() || !uiScene.isLoaded) return false;

        foreach (var root in uiScene.GetRootGameObjects())
        {
            var cam = root.GetComponentInChildren<Camera>();
            if (cam != null && cam.CompareTag("MainCamera"))
                return true;
        }

        return false;
    }
}