using UnityEngine;
using DG.Tweening;
using _Game.Games.WaterSort.Scripts.View;
using _Game.Games.WaterSort.Scripts.Model;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        private void DoTransfer(BottleView s, BottleView t) 
        {
            if(t.Model.IsSolved()){ DeselectCurrent(); return; }
            
            var sm = s.Model; 
            var tm = t.Model; 
            int c = sm.TopColor;
            
            if(tm.CanPush(c))
            {
                _localState = WaterSortLocalState.Pouring; 
                _isBusy = true;
                
                int amt = Mathf.Min(sm.GetCountSameTopColor(), tm.Capacity - tm.Count);
                _undoStack.Push(new MoveCommand(_activeBottles.IndexOf(s), _activeBottles.IndexOf(t), amt, c));

                s.AnimatePouring(t, amt, 
                    (p) => OnPouringStateChanged?.Invoke(p), 
                    () => { for(int i=0; i<amt; i++) tm.Push(sm.Pop()); }, 
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
            
            if(t.Model.IsSolved()) 
            { 
                t.PlaySolvedEffect(); 
                OnBottleSolved?.Invoke(); 
            } 
            
            CheckGameState(); 
            _isBusy = false; 
        }

        private void CheckGameState()
        {
            bool win = true;
            foreach(var b in _activeBottles) 
                if(!b.Model.IsEmpty && !b.Model.IsSolved()){ win = false; break; }
            
            if(win) 
            {
                HandleLevelWin();
                return;
            }

            if(IsDeadlock(true) && autoReshuffleOnDeadlock) 
            { 
                _localState = WaterSortLocalState.Reshuffling; 
                _isBusy = true; 
                DOVirtual.DelayedCall(2f, () => { if(this) ReshuffleBoard(); }); 
            }
            else 
            {
                _localState = WaterSortLocalState.Idle;
            }
        }

        private void HandleLevelWin()
        {
            _localState = WaterSortLocalState.Victory;
            if (gameHUD && LevelManager.HasInstance) 
                gameHUD.PlayWinSequence(LevelManager.Instance.GetCurrentLevelName());
            
            if(LevelManager.Instance) LevelManager.Instance.OnLevelWin(); 
            OnLevelWin?.Invoke();
        }

        private void ExecuteUndo()
        {
             if (_undoStack.Count == 0) return;
            
            MoveCommand cmd = _undoStack.Pop();
            BottleView source = _activeBottles[cmd.FromBottleIndex];
            BottleView target = _activeBottles[cmd.ToBottleIndex];

            for (int i = 0; i < cmd.Amount; i++) 
            { 
                target.Model.Pop(); 
                source.Model.UndoPush(cmd.ColorId); 
            }
            
            source.UpdateVisuals(); 
            target.UpdateVisuals();
            OnUndo?.Invoke(); 
        }
    }
}