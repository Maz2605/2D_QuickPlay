using UnityEngine;
using System.Collections.Generic;
using System;
using _Game.Core.Scripts.GameSystem;
using DG.Tweening; 
using _Game.Games.WaterSort.Scripts.View;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController : BaseGameController
    {
        [Header("--- CORE CONFIG & REFS ---")]
        [SerializeField] private BottleView bottlePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LayerMask bottleLayer;

        [Header("--- UI & SCENE ---")]
        [SerializeField] private WaterSortHUD gameHUD;
        
        [Header("--- SETTINGS ---")]
        [SerializeField] private bool autoReshuffleOnDeadlock = true; 

        // --- EVENTS ---
        public event Action OnBottleSelected;
        public event Action OnBottleDeselected;
        public event Action OnMoveInvalid;
        public event Action<bool> OnPouringStateChanged; 
        public event Action OnUndo;
        public event Action OnLevelWin;
        public event Action OnBottleSolved;

        // --- STATE & DATA ---
        private WaterSortLocalState _localState = WaterSortLocalState.Intro;
        private bool _isProcessingHint = false; 
        private bool _isBusy = false; 
        private bool _isPaused = false; 

        private BottleView _currentSelectedBottle;
        private List<BottleView> _activeBottles = new List<BottleView>();
        private Stack<MoveCommand> _undoStack = new Stack<MoveCommand>();
        private Camera _mainCamera;

        public override GlobalGameState CurrentGlobalState => GlobalGameState.Playing; // Giả sử

        private void Awake() 
        { 
            _mainCamera = Camera.main; 
        }

        private void Start()
        {
            RegisterUIEvents();
            RegisterInputEvents();
            
            _localState = WaterSortLocalState.Idle;
            Time.timeScale = 1f;

            if (LevelManager.HasInstance) UpdateHUDInfo();
        }

        private void OnDestroy()
        {
            UnregisterUIEvents();
            UnregisterInputEvents();

            transform.DOKill();
            Time.timeScale = 1f;
        }

        protected override void OnResetGameplay()
        {
            if (LevelManager.HasInstance)
            {
                Time.timeScale = 1f; 
                _isPaused = false; 
                _isBusy = false; 
                DeselectCurrent();
                
                LevelManager.Instance.OnClickResetGame();
                UpdateHUDInfo();
            }
        }
    }
}