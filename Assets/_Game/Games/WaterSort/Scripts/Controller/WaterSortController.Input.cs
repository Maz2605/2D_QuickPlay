using UnityEngine;
using DG.Tweening;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Games.WaterSort.Scripts.View;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        private void HandleTouchInput(Vector2 p) 
        {
             if(_currentState != WaterSortState.Idle || _isBusy || _isProcessingHint) return;

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
                if(!b.Model.IsEmpty) 
                { 
                    SelectBottle(b); 
                } 
            }
            else 
            {
                if(_currentSelectedBottle == b) 
                { 
                    DeselectCurrent(); 
                }
                else 
                {
                    if(b.Model.CanPush(_currentSelectedBottle.Model.TopColor)) 
                    {
                        DoTransfer(_currentSelectedBottle, b);
                    }
                    else 
                    { 
                        EventManager<WaterSortEventID>.Post(WaterSortEventID.MoveInvalid);
                        
                        _isBusy = true; 
                        _currentSelectedBottle.AnimateShakeError(() => { 
                            DeselectCurrent(); 
                            _isBusy = false;
                        }); 
                    }
                }
            }
        }

        private void SelectBottle(BottleView b)
        { 
            _currentSelectedBottle = b; 
            b.SetSelected(true); 
            EventManager<WaterSortEventID>.Post(WaterSortEventID.BottleSelected);
        }
        
        private void DeselectCurrent()
        { 
            if(_currentSelectedBottle)
            { 
                _currentSelectedBottle.SetSelected(false); 
                _currentSelectedBottle = null; 
                EventManager<WaterSortEventID>.Post(WaterSortEventID.BottleDeselected);
            } 
        }

        // --- TRANSFER & COMMAND LOGIC ---
        private void DoTransfer(BottleView s, BottleView t) 
        {
            if(t.Model.IsSolved()){ DeselectCurrent(); return; }
            
            var sm = s.Model; 
            var tm = t.Model; 
            int c = sm.TopColor;
            
            if(tm.CanPush(c))
            {
                ChangeState(WaterSortState.Pouring);
                _isBusy = true;
                
                int amt = Mathf.Min(sm.GetCountSameTopColor(), tm.Capacity - tm.Count);
                
                var command = new MoveCommand(sm, tm, amt, c);

                EventManager<WaterSortEventID>.Post(WaterSortEventID.PouringStarted);

                s.AnimatePouring(t, amt, 
                    (isPouring) => {  }, 
                    
                    () => { 
                        
                        _commandInvoker.ExecuteCommand(command); 
                    }, 
                    
                    () => { 
                        OnTransferComplete(s, t);
                    });
            }
        }

        private void OnTransferComplete(BottleView s, BottleView t)
        {
            s.UpdateVisuals(); 
            t.UpdateVisuals(); 
            DeselectCurrent(); 
            
            EventManager<WaterSortEventID>.Post(WaterSortEventID.PouringCompleted);

            if(t.Model.IsSolved()) 
            { 
                t.PlaySolvedEffect(); 
                EventManager<WaterSortEventID>.Post(WaterSortEventID.BottleSolved);
            } 
            
            _isBusy = false; 
            CheckGameState(); 
        }
    }
}