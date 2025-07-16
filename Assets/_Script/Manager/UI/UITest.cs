using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    private void Start()
    {   
        CheckCamera();
    }

    public void LoadFruitMerge()
    {
        GameLoader.Instance.LoadGame("FruitMergeGame");
    }
    public void BackToMainMenu()
    {
        GameLoader.Instance.BackToMainMenu();
    }

    public void LoadFlappyJump()
    {
        GameLoader.Instance.LoadGame("FlappyJumpGame");
    }
    
    public void LoadSudoku()
    {
        GameLoader.Instance.LoadGame("SudokuGame");
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
