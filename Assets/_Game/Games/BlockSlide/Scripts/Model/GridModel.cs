using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Games.BlockSlide.Scripts.Model
{
    public class GridModel
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private int [,] _board;
        private bool _isMoved;
        private int[] _lineBuffer;

        public GridModel(int width, int height)
        {
            Width = width;
            Height = height;
            _board = new int[width, height];
            
            int maxDim = width > height ? width : height;
            _lineBuffer = new int[maxDim];
        }

        public void ResetGame()
        {
            Array.Clear(_board, 0, _board.Length);
            SpawnRandomTile();
            SpawnRandomTile();
            
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.GameStart);
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.BoardUpdate);
        }

        
        
        public void MoveBlock(BlockMoveDirection direction)
        {
            _isMoved = false;

            switch (direction)
            {
                case BlockMoveDirection.Left:
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            _lineBuffer[x] = _board[x, y];
                        }
                        
                        ProcessBuffer(Width);

                        for (int x = 0; x < Width; x++)
                        {
                            if (_board[x, y] != _lineBuffer[y])
                            {
                                _board[x, y] = _lineBuffer[x];
                                _isMoved = true;
                            }
                        }
                    }
                    break;
                case BlockMoveDirection.Right:
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            _lineBuffer[x] = _board[Width - 1 - x , y];
                        }
                        
                        ProcessBuffer(Width);
                        
                        for (int x = 0; x < Width; x++)
                        {
                            if (_board[x, y] != _lineBuffer[Width - 1 - x])
                            {
                                _board[x, y] = _lineBuffer[Width - 1 - x];
                                _isMoved = true;
                            }
                        }
                    }
                    break;
                case BlockMoveDirection.Up:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            _lineBuffer[y] = _board[x, Height - 1 - y];
                        }
                        
                        ProcessBuffer(Height);

                        for (int y = 0; y < Height; y++)
                        {
                            if (_board[x, Height - 1 - y] != _lineBuffer[y])
                            {
                                _board[x, Height - 1 - y] = _lineBuffer[y];
                                _isMoved = true;
                            }
                        }
                    }
                    break;
                case BlockMoveDirection.Down:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            _lineBuffer[y] = _board[x, y];
                        }
                        
                        ProcessBuffer(Height);

                        for (int y = 0; y < Height; y++)
                        {
                            if (_board[x, y] != _lineBuffer[y])
                            {
                                _board[x, y] = _lineBuffer[y];
                                _isMoved = true;
                            }
                        }
                    }
                    break;
            }

            if (_isMoved)
            {
                SpawnRandomTile();
                EventManager<BlockSlideEventID>.Post(BlockSlideEventID.BoardUpdate);
            }

        }

        private void ProcessBuffer(int lenght)
        {
            int writePos = 0;
            int lastMergerPos = -1;

            for (int readPos = 0; readPos < lenght; readPos++)
            {
                int value = _lineBuffer[readPos];
                if(value == 0) continue;
                
                if(writePos > 0 && _lineBuffer[writePos - 1 ] == value && lastMergerPos != (writePos - 1) )
                {
                    int newValue = value * 2;
                    _lineBuffer[writePos - 1] = newValue;
                    
                    EventManager<BlockSlideEventID>.Post(BlockSlideEventID.ScoreUpdate, newValue);
                    lastMergerPos = writePos - 1;
                }
                else
                {
                    _lineBuffer[writePos] = value;
                    writePos++;
                }
            }

            for (int i = writePos; i < lenght; i++)
            {
                _lineBuffer[i] = 0;
            }
        }
        
        public void SpawnRandomTile()
        {
            List<Vector2Int> emptySpots = GetEmptySpots();
            if(emptySpots.Count == 0) return;
            
            Vector2Int spot = emptySpots[Random.Range(0, emptySpots.Count)];
            int value = Random.value < 0.1f ? 4 : 2;
            _board[spot.x, spot.y] = value;
        }

        
        private List<Vector2Int> GetEmptySpots()
        {
            List<Vector2Int> emptySpots = new List<Vector2Int>();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if(_board[i, j] == 0)
                        emptySpots?.Add(new Vector2Int(i, j));
                }
            }

            return emptySpots;
        }
        
        public int GetCell(int x, int y)
        {
            return _board[x, y];
        }
        
        public void LogBoard()
        {
            string log = "--- Board Updated ---\n";
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++) log += _board[x, y] + "\t";
                log += "\n";
            }

            Debug.Log(log);
        }
    }
}