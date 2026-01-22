using UnityEngine;
using _Game.Core.Scripts.Input;
using _Game.Games.WaterSort.Scripts.View;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        private void RegisterInputEvents()
        {
            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart += HandleTouchInput;
        }

        private void UnregisterInputEvents()
        {
            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart -= HandleTouchInput;
        }

        private void HandleTouchInput(Vector2 p) 
        {
             if(_localState != WaterSortLocalState.Idle || _isBusy || _isPaused || _isProcessingHint) return;

             Vector3 wp = _mainCamera.ScreenToWorldPoint(p);
             RaycastHit2D hit = Physics2D.Raycast(wp, Vector2.zero, Mathf.Infinity, bottleLayer);
             
             if(hit.collider != null) 
             { 
                 var b = hit.collider.GetComponentInParent<BottleView>(); 
                 if(b) OnBottleClicked(b); 
             }
             else if(_currentSelectedBottle != null) 
             {
                 DeselectCurrent();
             }
        }

        private void OnBottleClicked(BottleView b) 
        {
            if(b.Model.IsSolved()) return;

            if(_currentSelectedBottle == null) 
            { 
                if(!b.Model.IsEmpty) { SelectBottle(b); OnBottleSelected?.Invoke(); } 
            }
            else 
            {
                if(_currentSelectedBottle == b) 
                { 
                    DeselectCurrent(); OnBottleDeselected?.Invoke(); 
                }
                else 
                {
                    // Logic check valid move nằm ở file Logic
                    if(b.Model.CanPush(_currentSelectedBottle.Model.TopColor)) 
                    {
                        DoTransfer(_currentSelectedBottle, b);
                    }
                    else 
                    { 
                        OnMoveInvalid?.Invoke(); 
                        _isBusy = true; 
                        _currentSelectedBottle.AnimateShakeError(() => { 
                            DeselectCurrent(); 
                            _isBusy = false;
                        }); 
                    }
                }
            }
        }

        private void SelectBottle(BottleView b){ _currentSelectedBottle = b; b.SetSelected(true); }
        private void DeselectCurrent(){ if(_currentSelectedBottle){ _currentSelectedBottle.SetSelected(false); _currentSelectedBottle = null; } }
    }
}