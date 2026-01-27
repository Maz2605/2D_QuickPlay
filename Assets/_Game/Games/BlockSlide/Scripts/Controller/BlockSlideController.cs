using System;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.Command;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;
using _Game.Games.BlockSlide.Scripts.View;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public partial class BlockSlideController : BaseGameController 
    {
        [SerializeField] private BlockSlideConfigSO config;
        [SerializeField] private BoardView boardView;
        [SerializeField] private BlockSlideHUD blockSlideHUD;
        [SerializeField] private float minSwipeDistance = 50f;
        
        private BlockSlideState _currentState;
        private GridModel _gridModel;
        private CommandInvoker _commandInvoker;
        private BlockSlideScoreManager _scoreManager;
        
        private Vector2 _startPosition;
        private bool _isSwipeProcessing;
        
        private void Start()
        {
            _gridModel = new GridModel(config.boardWith, config.boardHeight);
            _commandInvoker = new CommandInvoker();
            _scoreManager = new BlockSlideScoreManager(config.gameProfile.id);
            
            boardView.Initialize(_gridModel);
            if (blockSlideHUD != null)
            {
                blockSlideHUD.Initialize();
                blockSlideHUD.OnHomeClicked += RequestBackHome;
                blockSlideHUD.OnPauseClicked += RequestPause;
                blockSlideHUD.OnReplayClicked += RequestReplay;
                blockSlideHUD.OnUndoClicked += RequestUndo;
                blockSlideHUD.OnReplayWithoutPopupClicked += RequestReplayWithoutConfirmation;
                blockSlideHUD.OnHomeWithoutPopupClicked += RequestBackHomeWithoutConfirmation;
            }
            
            RestartGame();
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchStart += HandleTouchStart;
                InputManager.Instance.OnTouchEnd += HandleTouchEnd;
            }
            
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.BoardUpdate, OnBoardUpdate);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchStart -= HandleTouchStart;
                InputManager.Instance.OnTouchEnd -= HandleTouchEnd;
            }
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.BoardUpdate,OnBoardUpdate);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
        }

        private void OnDestroy()
        {
            _scoreManager?.Save();
            if (blockSlideHUD != null)
            {
                blockSlideHUD.OnHomeClicked -= RequestBackHome;
                blockSlideHUD.OnPauseClicked -= RequestPause;
                blockSlideHUD.OnReplayClicked -= RequestReplay;
                blockSlideHUD.OnUndoClicked -= RequestUndo;
            }
        }

        // private void Update()
        // {
        //     if(Input.GetKeyDown(KeyCode.A))
        //         OnGameOver();
        // }
    }
}