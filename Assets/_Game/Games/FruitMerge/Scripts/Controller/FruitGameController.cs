using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Core.Scripts.SkillSystem;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public partial class FruitGameController : BaseGameController
    {
        [Header("References")] 
        [SerializeField] private FruitMergeGameConfigSO config;
        [SerializeField] private FruitSpawner spawner;
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Transform fruitContainer;
        [SerializeField] private GameObject mergeEffectPrefab;

        [Header("Components")]
        [SerializeField] private FruitDeadZone deadZone; 
        [SerializeField] private FruitGameHUD gameHUD;
        
        private FruitMergeState _currentState;
        
        private FruitScoreManager _scoreManager;
        private readonly List<FruitUnit> _activeFruits = new List<FruitUnit>(); 
        
        
        private void Start()
        {
            _scoreManager = new FruitScoreManager(config.gameID);
            
            spawner.Initialize(config, OnSpawnRequest);

            
            
            if (gameHUD)
            {
                gameHUD.Initialize();
                
                gameHUD.OnPauseClicked += RequestPause; 
                gameHUD.OnReplayClicked += RequestReplay; 
                gameHUD.OnHomeClicked += RequestBackHome; 
                gameHUD.OnHomeWithoutPopupClicked += RequestBackHomeWithoutConfirmation;
                gameHUD.OnReplayWithoutPopupClicked += RequestReplayWithoutConfirmation;
            }

            StartGame();
        }

        private void OnEnable()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.OnTouchStart += HandleTouchStart;
                InputManager.Instance.OnTouchMove += HandleTouchMove;
                InputManager.Instance.OnTouchEnd += HandleTouchEnd;
            }

            if (deadZone)
            {
                deadZone.OnGameOverTriggered += HandleGameOver;
                deadZone.OnDangerTimerUpdate += HandleDangerUpdate;
            }
            
            EventManager<FruitMergeEventID>.AddListener<FruitCollisionPayload>(
                FruitMergeEventID.FruitCollision, OnFruitCollisionReceived);
        }

        private void OnDisable()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.OnTouchStart -= HandleTouchStart;
                InputManager.Instance.OnTouchMove -= HandleTouchMove;
                InputManager.Instance.OnTouchEnd -= HandleTouchEnd;
            }

            if (deadZone)
            {
                deadZone.OnGameOverTriggered -= HandleGameOver;
                deadZone.OnDangerTimerUpdate -= HandleDangerUpdate;
            }
            
            EventManager<FruitMergeEventID>.RemoveListener<FruitCollisionPayload>(
                FruitMergeEventID.FruitCollision, OnFruitCollisionReceived);
        }

        private void OnDestroy()
        {
            _scoreManager?.Save();
            
            if (gameHUD)
            {
                gameHUD.OnPauseClicked -= RequestPause;
                gameHUD.OnReplayClicked -= RequestReplay;
                gameHUD.OnHomeClicked -= RequestBackHome;
                gameHUD.OnHomeWithoutPopupClicked -= RequestBackHomeWithoutConfirmation;
                gameHUD.OnReplayWithoutPopupClicked -= RequestReplayWithoutConfirmation;
            }
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.A))
                HandleGameOver();
        }
    }
}