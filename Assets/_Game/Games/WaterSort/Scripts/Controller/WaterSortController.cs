using UnityEngine;
using System.Collections.Generic;
using System;
using _Game.Core.Scripts.GameSystem;
using DG.Tweening; 
using _Game.Games.WaterSort.Scripts.View;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling; 

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class WaterSortController : BaseGameController
    {
        [Header("--- CORE CONFIG & REFS ---")]
        [SerializeField] private BottleView bottlePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LayerMask bottleLayer;

        [Header("--- UI & SCENE ---")]
        [SerializeField] private WaterSortHUD gameHUD; // View Reference
        
        [Header("--- SETTINGS ---")]
        [SerializeField] private bool autoReshuffleOnDeadlock = true; 

        // --- GAMEPLAY EVENTS (AUDIO/VFX LISTENER) ---
        public event Action OnBottleSelected;
        public event Action OnBottleDeselected;
        public event Action OnMoveInvalid;
        public event Action<bool> OnPouringStateChanged; 
        public event Action OnUndo;
        public event Action OnLevelWin;
        public event Action OnBottleSolved;

        // --- STATE ---
        private WaterSortLocalState _localState = WaterSortLocalState.Intro;
        private bool _isProcessingHint = false; 
        private bool _isBusy = false; 
        private bool _isPaused = false; 

        // --- DATA ---
        private BottleView _currentSelectedBottle;
        private List<BottleView> _activeBottles = new List<BottleView>();
        private Stack<MoveCommand> _undoStack = new Stack<MoveCommand>();
        private Camera _mainCamera;

        private void Awake() 
        { 
            _mainCamera = Camera.main; 
        }

        private void Start()
        {
            if (gameHUD)
            {
                gameHUD.Initialize();
                gameHUD.OnUndoClicked += HandleUndoRequest;
                gameHUD.OnReplayClicked += HandleReplayRequest;
                gameHUD.OnHintClicked += HandleHintRequest;
                gameHUD.OnPauseClicked += HandlePauseRequest;
                gameHUD.OnHomeClicked += HandleHomeRequest;
                
                gameHUD.OnWinAnimationCovered += HandleLoadNextLevel;
            }

            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart += HandleTouchInput;
            
            _localState = WaterSortLocalState.Idle;
            Time.timeScale = 1f;

            if (LevelManager.HasInstance)
            {
                UpdateHUDInfo();
            }
        }

        private void OnDestroy()
        {
            if (gameHUD)
            {
                gameHUD.OnUndoClicked -= HandleUndoRequest;
                gameHUD.OnReplayClicked -= HandleReplayRequest;
                gameHUD.OnHintClicked -= HandleHintRequest;
                gameHUD.OnPauseClicked -= HandlePauseRequest;
                gameHUD.OnHomeClicked -= HandleHomeRequest;
                gameHUD.OnWinAnimationCovered -= HandleLoadNextLevel;
            }

            if (InputManager.Instance != null) InputManager.Instance.OnTouchStart -= HandleTouchInput;
            transform.DOKill();
            Time.timeScale = 1f;
        }

        // --- EVENT HANDLERS (Xử lý yêu cầu từ View) ---

        private void HandleUndoRequest()
        {
            if (_isPaused || _localState != WaterSortLocalState.Idle || _undoStack.Count == 0 || _isBusy) return;
            
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

        private void HandleReplayRequest()
        {
            RequestReplay();
        }

        private void HandleHintRequest()
        {
            if (!_isPaused && !_isBusy) ShowHint();
        }

        private void HandlePauseRequest()
        {
            RequestPause();
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            if (gameHUD) gameHUD.SetPauseState(_isPaused);
        }

        private void HandleHomeRequest()
        {
            RequestBackHome();
        }

        private void HandleLoadNextLevel()
        {
            if (LevelManager.HasInstance)
            {
                LevelManager.Instance.LoadNextLevel();
                UpdateHUDInfo(); 
            }
        }

        private void UpdateHUDInfo()
        {
            if (gameHUD && LevelManager.HasInstance)
            {
                gameHUD.UpdateLevelName(LevelManager.Instance.GetCurrentLevelName());
            }
        }

        // --- GAMEPLAY CORE LOGIC ---
        protected override void OnResetGameplay()
        {
            if (LevelManager.HasInstance)
            {
                Time.timeScale = 1f; 
                _isPaused = false; 
                _isBusy = false; 
                DeselectCurrent();
                
                LevelManager.Instance.OnClickResetGame();
                UpdateHUDInfo();
            }
        }

        public void LoadLevel(LevelConfigSO levelData)
        {
            _localState = WaterSortLocalState.Intro; 
            _undoStack.Clear(); 
            ClearCurrentLevel();

            if (gameHUD)
            {
                string levelName = string.IsNullOrEmpty(levelData.nameLevel) ? "Level" : levelData.nameLevel;
                gameHUD.UpdateLevelName(levelName);
            }

            // Spawn Logic
            int currentBottleIndex = 0;
            Vector3 currentRowPos = spawnPoint.position;
            for (int rowIndex = 0; rowIndex < levelData.bottlesPerRow.Count; rowIndex++)
            {
                int countInRow = levelData.bottlesPerRow[rowIndex];
                float startX = -(countInRow - 1) * levelData.spacingX / 2f;
                for (int i = 0; i < countInRow; i++) 
                {
                    if (currentBottleIndex >= levelData.bottles.Count) break;
                    Vector3 spawnPos = currentRowPos; 
                    spawnPos.x += startX + (i * levelData.spacingX);
                    SpawnSingleBottle(spawnPos, currentBottleIndex, levelData);
                    currentBottleIndex++;
                }
                currentRowPos.y -= levelData.spacingY;
            }
            _localState = WaterSortLocalState.Idle;
        }

        private void SpawnSingleBottle(Vector3 pos, int index, LevelConfigSO levelData) 
        {
             BottleView bot = PoolingManager.Instance.Spawn(bottlePrefab, pos, Quaternion.identity, transform);
             BottleModel model = new BottleModel(levelData.bottleCapacity);
             foreach (int color in levelData.bottles[index].colors) model.Push(color);
             bot.Initialize(model); 
             bot.name = $"Bottle_{index}"; 
             _activeBottles.Add(bot);
        }

        private void ClearCurrentLevel() 
        { 
            foreach (var b in _activeBottles) 
            { 
                if(b)
                {
                    b.transform.DOKill(); 
                    b.SetSelected(false); 
                    PoolingManager.Instance.Despawn(b.gameObject);
                } 
            } 
            _activeBottles.Clear(); 
            _currentSelectedBottle=null; 
            _isProcessingHint=false; 
            _isBusy=false; 
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
                    });
            }
        }

        private void CheckGameState()
        {
            bool win = true;
            foreach(var b in _activeBottles) 
                if(!b.Model.IsEmpty && !b.Model.IsSolved()){ win = false; break; }
            
            if(win) 
            {
                _localState = WaterSortLocalState.Victory;
                
                if (gameHUD && LevelManager.HasInstance) 
                {
                    gameHUD.PlayWinSequence(LevelManager.Instance.GetCurrentLevelName());
                }
                if(LevelManager.Instance) LevelManager.Instance.OnLevelWin(); 
                OnLevelWin?.Invoke();
                
                
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

        // --- SUPPORT LOGIC (HINT, RESHUFFLE) ---

        public void ShowHint()
        {
            if (_localState != WaterSortLocalState.Idle || _isBusy || _isProcessingHint) return;
            _isProcessingHint = true;
            
            foreach (var s in _activeBottles) 
            {
                if (s.Model.IsEmpty || s.Model.IsSolved()) continue;
                foreach (var t in _activeBottles) 
                {
                    if (s == t || t.Model.IsSolved()) continue;
                    if (t.Model.CanPush(s.Model.TopColor)) 
                    {
                        if (t.Model.IsEmpty && s.Model.IsUniformColor()) continue;
                        if ((t.Model.Capacity - t.Model.Count) < s.Model.GetCountSameTopColor()) continue;
                        
                        // FOUND HINT
                        s.transform.DOJump(s.transform.position, 0.5f, 1, 0.5f)
                            .OnComplete(() => _isProcessingHint = false);
                        return;
                    }
                }
            }
            // NO MOVE -> RESHUFFLE
            ReshuffleBoard(); 
            _isProcessingHint = false;
        }
        
        private bool IsDeadlock(bool strict)
        {
            foreach (var s in _activeBottles) {
                if (s.Model.IsEmpty || s.Model.IsSolved()) continue;
                foreach (var t in _activeBottles) {
                    if (s == t || t.Model.IsSolved()) continue;
                    if (t.Model.CanPush(s.Model.TopColor)) {
                        if (strict) {
                            if (t.Model.IsEmpty && s.Model.IsUniformColor()) continue;
                            if ((t.Model.Capacity - t.Model.Count) < s.Model.GetCountSameTopColor()) continue;
                        }
                        return false; 
                    }
                }
            }
            return true; 
        }

        private void ReshuffleBoard()
        {
            _localState = WaterSortLocalState.Reshuffling;
            List<int> allLiquids = new List<int>();
            foreach (var bot in _activeBottles) allLiquids.AddRange(bot.Model.ClearAndGetAllLiquids());

            bool success = false;
            for (int i = 0; i < 100; i++) {
                ShuffleList(allLiquids);
                if (TryDistributeRandomly(allLiquids)) {
                    if (!IsDeadlock(true)) { success = true; break; }
                }
                foreach (var bot in _activeBottles) bot.Model.ClearAndGetAllLiquids();
            }

            if (success) {
                foreach (var bot in _activeBottles) { 
                    bot.UpdateVisuals(); 
                    bot.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.5f); 
                }
                _undoStack.Clear(); 
                DOVirtual.DelayedCall(0.6f, () => { 
                    _localState = WaterSortLocalState.Idle; 
                    _isBusy = false; 
                });
            } else {
                if(LevelManager.Instance) LevelManager.Instance.OnClickResetGame();
            }
        }

        private bool TryDistributeRandomly(List<int> liquids) {
            Queue<int> pool = new Queue<int>(liquids);
            List<List<int>> tempBottles = new List<List<int>>();
            for(int i=0; i<_activeBottles.Count; i++) tempBottles.Add(new List<int>());
            while(pool.Count > 0) {
                int color = pool.Dequeue();
                List<int> availableIndices = new List<int>();
                for(int i=0; i<_activeBottles.Count; i++) if(tempBottles[i].Count < _activeBottles[i].Model.Capacity) availableIndices.Add(i);
                if(availableIndices.Count == 0) return false;
                tempBottles[UnityEngine.Random.Range(0, availableIndices.Count)].Add(color);
            }
            for(int i=0; i<_activeBottles.Count; i++) _activeBottles[i].Model.ForceSetLiquids(tempBottles[i]);
            return true;
        }

        private void ShuffleList<T>(List<T> list) { 
            int n = list.Count; 
            while (n > 1) { 
                n--; int k = UnityEngine.Random.Range(0, n + 1); 
                (list[k], list[n]) = (list[n], list[k]); 
            } 
        }

        public override GlobalGameState CurrentGlobalState { get; }
    }
}