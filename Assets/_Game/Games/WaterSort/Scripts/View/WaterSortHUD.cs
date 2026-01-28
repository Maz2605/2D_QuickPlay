using System;
using _Game.Core.Scripts.UI;
using _Game.Core.Scripts.UI.Base;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.Games.WaterSort.Scripts.View
{
    public class WaterSortHUD : BaseHUD
    {
        [Header("--- WATER SORT SPECIFIC ---")]
        [Header("Dependencies")]
        [SerializeField] private LevelManager levelManager;

        [Header("Skill Buttons")]
        [SerializeField] private Button btnUndo;
        [SerializeField] private Button btnHint;

        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI txtLevelDisplayName; 

        [Header("Components")]
        [SerializeField] private LevelCompleteResult levelCompletePanel;

        public Action OnUndoClicked;
        public Action OnHintClicked;
        public Action OnWinAnimationCovered; 

        private string _currentLevelName = "Level 1";

        public override void Initialize()
        {
            base.Initialize();
            BindButton(btnUndo, () => OnUndoClicked?.Invoke());
            BindButton(btnHint, () => OnHintClicked?.Invoke());

            if (levelManager == null)
            {
                levelManager = GetComponentInParent<LevelManager>();
            }

            SyncLevelName();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager<WaterSortEventID>.AddListener<WaterSortState>(WaterSortEventID.GameStateChanged, OnGameStateChanged);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.LevelWin, OnLevelWin);
            EventManager<WaterSortEventID>.AddListener<string>(WaterSortEventID.LevelLoaded, OnLevelLoaded);
            
            SyncLevelName();
        }

        protected override void OnDisable()
        {
            EventManager<WaterSortEventID>.RemoveListener<WaterSortState>(WaterSortEventID.GameStateChanged, OnGameStateChanged);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.LevelWin, OnLevelWin);
            EventManager<WaterSortEventID>.RemoveListener<string>(WaterSortEventID.LevelLoaded, OnLevelLoaded);
            base.OnDisable();
        }

        // --- HELPER FIX SYNC ---
        private void SyncLevelName()
        {
            if (levelManager != null)
            {
                OnLevelLoaded(levelManager.GetCurrentLevelName());
            }
        }

        // --- EVENT HANDLERS ---

        private void OnLevelLoaded(string levelName)
        {
            _currentLevelName = levelName;
            
            if (txtLevelDisplayName)
            {
                txtLevelDisplayName.text = levelName;
            }
            
            if (gamePlayPanel.alpha < 0.9f) ShowPanel(gamePlayPanel, true);
        }

        private void OnLevelWin()
        {
            ShowPanel(gamePlayPanel, false);

            if (levelCompletePanel != null)
            {
                levelCompletePanel.Show(
                    _currentLevelName, 
                    onCoveredCallback: () => 
                    {
                        OnWinAnimationCovered?.Invoke();
                    }
                );
            }
        }

        private void OnGameStateChanged(WaterSortState state)
        {
            switch (state)
            {
                case WaterSortState.Intro:
                case WaterSortState.Idle:
                case WaterSortState.Pouring:
                case WaterSortState.Reshuffling:
                    if (gamePlayPanel.alpha < 0.9f) ShowPanel(gamePlayPanel, true);
                    break;

                case WaterSortState.Victory:
                    ShowPanel(gamePlayPanel, false);
                    break;
                    
                case WaterSortState.Paused:
                    break;
            }
        }
    }
}