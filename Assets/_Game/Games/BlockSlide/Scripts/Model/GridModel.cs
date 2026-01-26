using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Controller;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Games.BlockSlide.Scripts.Model
{
    public class GridModel
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CurrentScore { get; private set; }
        
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
        
        public void MoveBlock(BlockMoveDirection direction)
        {
            _isMoved = false;

            switch (direction)
            {
                case BlockMoveDirection.Up:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            _lineBuffer[y] = _board[x, Height - y - 1];
                        }
                        
                        ProcessBuffer(Height);

                        for (int y = 0; y < Height; y++)
                        {
                            if (_board[x, Height - 1 - y] != _lineBuffer[y])
                            {
                                _board[x, Height - y - 1] = _lineBuffer[y];
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
                            if (_board[x, y] != _lineBuffer[x])
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
                        for (int i = 0; i < Width; i++)
                        {
                            _lineBuffer[i] = _board[Width - 1 - i, y];
                        }
                        
                        ProcessBuffer(Width);

                        for (int x = 0; x < Width; x++)
                        {
                            if (_board[Width - 1 - x, y] != _lineBuffer[x])
                            {
                                _board[Width - 1 - x, y] = _lineBuffer[x];
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
                
                if(CheckIsGameOver())
                    EventManager<BlockSlideEventID>.Post(BlockSlideEventID.GameOver);
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

                    CurrentScore += newValue;
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

        private bool CheckIsGameOver()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_board[x, y] == 0) return false;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int current = _board[x, y];
                    if (x < Width - 1 && current == _board[x + 1, y]) return false;
                    
                    if(y < Height - 1 && current == _board[x, y + 1]) return false;
                }
            }

            return true;
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


        public BoardSnapshot CaptureState()
        {
            return new BoardSnapshot((int[,])_board.Clone(), CurrentScore);
        }

        public void RestoreState(BoardSnapshot snapshot)
        {
            _board = (int[,])snapshot.BoardData.Clone();
            CurrentScore = snapshot.Score;
            
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.BoardUpdate);
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.ScoreUpdate, CurrentScore);
        }

        public void ResetGame()
        {
            Array.Clear(_board, 0, _board.Length);
            CurrentScore = 0;
            SpawnRandomTile();
            SpawnRandomTile();
            
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.GameStart);
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.BoardUpdate);
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.ScoreUpdate);
        }
        
        public bool IsLastMoveChanged => _isMoved;
    }
}