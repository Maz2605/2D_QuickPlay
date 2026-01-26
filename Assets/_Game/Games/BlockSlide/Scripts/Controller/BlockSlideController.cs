using System;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.Command;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;
using _Game.Games.BlockSlide.Scripts.View;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public partial class BlockSlideController : MonoBehaviour 
    {
        private GridModel _gridModel;
        [SerializeField] private BlockSlideConfigSO config;
        [SerializeField] private BoardView boardView;
        

        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.GameOver, OnGameOver);

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchStart += HandleTouchStart;
                InputManager.Instance.OnTouchEnd += HandleTouchEnd;
            }
            else
            {
                Debug.LogWarning("No InputManager found");
            }
        }

        

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchStart -= HandleTouchStart;
                InputManager.Instance.OnTouchEnd -= HandleTouchEnd;
            }
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.GameOver, OnGameOver);
        }

        private void Start()
        {
            Debug.Log("Game started");
            _commandInvoker = new CommandInvoker(undoLimit);
            StartNewGame();
        }

        void StartNewGame()
        {
            _gridModel = new GridModel(config.boardWith, config.boardHeight);
            boardView.Initialize(_gridModel);
            _gridModel.ResetGame();
        }
        
        private void OnBoardUpdated()
        {
            // _gridModel.LogBoard();
        }
        private void OnGameOver()
        {
            Debug.LogWarning("===Game over====");
        }

        private void Update()
        {
            if (_gridModel == null) return;

            if (_gridModel == null) return;

            // Test Input - SỬA LẠI DÙNG EXECUTE MOVE ĐỂ LƯU HISTORY
            if (Input.GetKeyDown(KeyCode.LeftArrow)) ExecuteMove(BlockMoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) ExecuteMove(BlockMoveDirection.Right);
            if (Input.GetKeyDown(KeyCode.UpArrow)) ExecuteMove(BlockMoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) ExecuteMove(BlockMoveDirection.Down);
    
            if (Input.GetKeyDown(KeyCode.Space)) StartNewGame();
    
            // Undo / Redo Shortcuts
            if (Input.GetKeyDown(KeyCode.U)) 
            {
                Debug.Log("Undo!");
                _commandInvoker.Undo();
            }

            if (Input.GetKeyDown(KeyCode.R)) 
            {
                Debug.Log("Redo!");
                _commandInvoker.Redo();
            }
        }
    }
}