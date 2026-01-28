using System;
using UnityEngine;
using System.Collections.Generic;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.Command;
using _Game.Games.WaterSort.Scripts.Config;
using DG.Tweening; 
using _Game.Games.WaterSort.Scripts.View;
using UnityEngine.Serialization;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController : BaseGameController
    {
        [Header("--- CORE CONFIG & REFS ---")]
        [SerializeField] private BottleView bottlePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LayerMask bottleLayer;

        [Header("--- MODULE DEPENDENCIES ---")]
        [SerializeField] private LevelManager levelManager;

        [Header("--- UI ---")]
        [SerializeField] private WaterSortHUD gameHUD;
        
        [Header("--- SETTINGS ---")]
        [SerializeField] private bool autoReshuffleOnDeadlock = true; 

        private WaterSortState _currentState = WaterSortState.Intro;
        private Camera _mainCamera;
        
        private BottleView _currentSelectedBottle;
        private List<BottleView> _activeBottles = new List<BottleView>();
        
        private CommandInvoker _commandInvoker; 
        
        private bool _isProcessingHint = false; 
        private bool _isBusy = false; 

        private void Awake() 
        { 
            _mainCamera = Camera.main; 

            if (levelManager == null)
            {
                levelManager = GetComponentInParent<LevelManager>();
                
                if (levelManager == null) levelManager = GetComponentInChildren<LevelManager>();
                if (levelManager == null)
                {
                    Debug.LogError("[WaterSortController] CRITICAL: Không tìm thấy LevelManager! Game sẽ không chạy.");
                    this.enabled = false;
                    return;
                }
            }
        }

        private void Start()
        {
            _commandInvoker = new CommandInvoker(); 

            if (gameHUD)
            {
                gameHUD.Initialize();
                gameHUD.OnUndoClicked += RequestUndo;
                gameHUD.OnReplayClicked += RequestReplay;
                gameHUD.OnHintClicked += RequestHint;
                gameHUD.OnPauseClicked += RequestPause;
                gameHUD.OnHomeClicked += RequestBackHome;
                gameHUD.OnWinAnimationCovered += LoadNextLevel;
            }
            
            if (levelManager != null) 
            {
                levelManager.InitGame(this);
            }
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart += HandleTouchInput;
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart -= HandleTouchInput;
        }

        private void OnDestroy()
        {
            transform.DOKill();
            
            if (gameHUD)
            {
                gameHUD.OnUndoClicked -= RequestUndo;
                gameHUD.OnReplayClicked -= RequestReplay;
                gameHUD.OnHintClicked -= RequestHint;
                gameHUD.OnPauseClicked -= RequestPause;
                gameHUD.OnHomeClicked -= RequestBackHome;
                gameHUD.OnWinAnimationCovered -= LoadNextLevel;
            }
        }
        
        
    }
}