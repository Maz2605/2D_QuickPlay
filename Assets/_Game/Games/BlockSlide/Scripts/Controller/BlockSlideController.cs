using System;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public class BlockSlideController : MonoBehaviour 
    {
        private GridModel _gridModel;

        [SerializeField]
        private BlockSlideConfigSO config;

        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
        }

        private void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
        }

        private void Start()
        {
            Debug.Log("Game started");
            StartNewGame();
        }

        void StartNewGame()
        {
            _gridModel = new GridModel(config.boardWith, config.boardHeight);
            _gridModel.ResetGame();
        }
        
        private void OnBoardUpdated()
        {
            _gridModel.LogBoard();
        }

        private void Update()
        {
            if (_gridModel == null) return;

            // Test Input
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _gridModel.MoveBlock(BlockMoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) _gridModel.MoveBlock(BlockMoveDirection.Right);
            if (Input.GetKeyDown(KeyCode.UpArrow)) _gridModel.MoveBlock(BlockMoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) _gridModel.MoveBlock(BlockMoveDirection.Down);
            
            if (Input.GetKeyDown(KeyCode.Space)) StartNewGame();
        }
    }
}