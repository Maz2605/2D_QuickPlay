using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public partial class BlockSlideController
    {
        [Header("Input Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        
        private Vector2 _startPosition;
        private bool _isSwipeProcessing;
        private void HandleTouchStart(Vector2 position)
        {
            _startPosition = position;
            _isSwipeProcessing = true;
        }

        private void HandleTouchEnd(Vector2 endPosition)
        {
            if(!_isSwipeProcessing || _gridModel == null) return;
            
            Vector2 swipeVector = endPosition - _startPosition;

            if (swipeVector.magnitude < minSwipeDistance)
            {
                _isSwipeProcessing = false;
                return;
            }

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                if (swipeVector.x > 0)
                {
                    Debug.Log("Swipe Right");
                    // _gridModel.MoveBlock(BlockMoveDirection.Right);
                    ExecuteMove(BlockMoveDirection.Right);
                }
                else
                {
                    Debug.Log("Swipe Left");
                    // _gridModel.MoveBlock(BlockMoveDirection.Left);
                    ExecuteMove(BlockMoveDirection.Left);
                }
            }
            else
            {
                if (swipeVector.y > 0)
                {
                    Debug.Log("Swipe Up");
                    // _gridModel.MoveBlock(BlockMoveDirection.Up);
                    ExecuteMove(BlockMoveDirection.Up);
                }
                else
                {
                    Debug.Log("Swipe Down");
                    // _gridModel.MoveBlock(BlockMoveDirection.Down);
                    ExecuteMove(BlockMoveDirection.Down);
                }
            }
            
            _isSwipeProcessing = false;
        }
    }
}