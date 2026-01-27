using UnityEngine;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BoardView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RectTransform rectTransform; 
        [SerializeField] private BlockView blockViewPrefab;
        [SerializeField] private BlockColorConfigSO colorConfig;

        private List<BlockView> _activeBlocks = new List<BlockView>();
        
        private int[,] _oldBoardState;
        
        private int _width;
        private int _height;

        public void Initialize(GridModel model)
        {
            _width = model.Width;
            _height = model.Height;
            _oldBoardState = new int[_width, _height];

            ClearBoard();

            for (int i = 0; i < _width * _height; i++)
            {
                SpawnBlock();
            }

            UpdateBoard(model);
        }

        private void SpawnBlock()
        {
            var block = PoolingManager.Instance.Spawn(
                blockViewPrefab, 
                Vector3.zero, 
                Quaternion.identity, 
                rectTransform
            );
            
            block.gameObject.SetActive(true);
            block.transform.localScale = Vector3.one;
            
            _activeBlocks.Add(block);
        }

        public void UpdateBoard(GridModel model)
        {
            int index = 0;

            
            for (int y = _height - 1; y >= 0; y--) 
            {
                for (int x = 0; x < _width; x++)
                {
                    if (index >= _activeBlocks.Count) break;

                    BlockView view = _activeBlocks[index];
                    int newValue = model.GetCell(x, y);
                    int oldValue = _oldBoardState[x, y];

                    bool isSpawning = false;
                    bool isMerging = false;

                    if (newValue != oldValue)
                    {
                        if (oldValue == 0 && newValue > 0)
                        {
                            isSpawning = true;
                        }
                        else if (newValue > oldValue && oldValue != 0)
                        {
                            isMerging = true;
                        }
                    }

                    view.SetData(newValue, colorConfig, isSpawning, isMerging);

                    // Lưu lại trạng thái mới
                    _oldBoardState[x, y] = newValue;
                    
                    // Tăng index để lấy BlockView tiếp theo trong list
                    index++;
                }
            }
        }

        private void ClearBoard()
        {
            foreach (var block in _activeBlocks)
            {
                if (block != null)
                {
                    block.ResetView();
                    PoolingManager.Instance.Despawn(block.gameObject);
                }
            }
            _activeBlocks.Clear();
        }
    }
}