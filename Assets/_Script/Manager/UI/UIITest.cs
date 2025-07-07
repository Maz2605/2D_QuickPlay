using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIITest : MonoBehaviour
{
    private void Start()
    {   
        CheckCamera();
    }
    
    void CheckCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            if (canvas.worldCamera == null || canvas.worldCamera != Camera.main)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
