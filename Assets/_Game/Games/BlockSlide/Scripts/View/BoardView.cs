using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BoardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BlockView blockPrefab;
        [SerializeField] private Transform gridContainer;
        [SerializeField] private BlockColorConfigSO blockColorConfig;
        
        private List<BlockView> _blocks = new List<BlockView>();
        private GridModel _modelReference;

        public void Initialize(GridModel model)
        {
            _modelReference = model;

            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }
            _blocks.Clear(); 
            GridLayoutGroup layout = gridContainer.GetComponent<GridLayoutGroup>();
            if (layout != null)
            {
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = model.Width;
            }

            int totalCell = model.Height * model.Width;
            for (int i = 0; i < totalCell; i++)
            {
                BlockView block = Instantiate(blockPrefab, gridContainer);
                
                block.SetData(0, blockColorConfig);
                _blocks.Add(block);
            }

            RedrawBoard();

        }
        private void RedrawBoard()
        {
            if (_modelReference == null) return;

            int index = 0;
            for (int y = _modelReference.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < _modelReference.Width; x++)
                {
                    int value = _modelReference.GetCell(x, y);
                    if (index < _blocks.Count)
                    {
                        _blocks[index].SetData(value, blockColorConfig);
                    }
                    index++;
                }
            }
        }

        private void OnBoardUpdated()
        {
            RedrawBoard();
        }

        private void OnGameStarted()
        {
            RedrawBoard();
        }
        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.GameStart, OnGameStarted);
        }

        private void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.BoardUpdate, OnBoardUpdated);
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.GameStart, OnGameStarted);
        }
        
    }
}