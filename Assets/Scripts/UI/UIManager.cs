using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using Zenject;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using cherrydev;

namespace ReaperGS
{
    public class UIManager : MonoBehaviour
    {
        public event Action OnFadeOutAtStart;

        [SerializeField] private Image _cursor;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _actionText;
        [SerializeField] private CanvasGroup _infoPanalCanvasGroup;

        [SerializeField] private GameObject _finishPanal;

        [Header("FadingImage")]
        [field: SerializeField] public float DefaultFadeTimeMultiplier { get; private set; }
        [SerializeField] private float _beforeCutSceneFading = 0.3f;

        [SerializeField] private Image _backgroundFadeImage;
        [SerializeField] private TMP_Text _pressAnyKeyToStartText;
        [SerializeField] private TMP_Text _littlelaterText;
        [SerializeField] private AnimationCurve _fadeOutCurve;
        [SerializeField] private AnimationCurve _fadeInCurve;

        [SerializeField] private MoneyPopUp _moneyPopUp;

        private PlayerInteractions _playerInteractions;
        private GameManager _gameManager;
        private PlayerInput _playerInput;
        private DialogBehaviour _dialogueBehaiviour;

        [Inject]
        private void Construct(FPSCameraController fPSCameraController, GameManager gameManager, PlayerInput playerInput, DialogBehaviour dialogBehaviour)
        {
            _playerInteractions = fPSCameraController.GetComponent<PlayerInteractions>();
            _gameManager = gameManager;
            _playerInput = playerInput;
            _dialogueBehaiviour = dialogBehaviour;
        }

        private void OnEnable()
        {
            _playerInteractions.OnInteractableOver += ShowInfoPanal;
            _playerInteractions.OnInteractionReset += HideInfoPanal;

            _gameManager.OnNewGameStateEntered += HandleGameStates;

            _dialogueBehaiviour.OnDialogueStarted += HideCursorPoint;
            _dialogueBehaiviour.OnDialogueFinished += ShowCursorPoint;
        }

        private void OnDisable()
        {
            _playerInteractions.OnInteractableOver -= ShowInfoPanal;
            _playerInteractions.OnInteractionReset -= HideInfoPanal;

            _gameManager.OnNewGameStateEntered -= HandleGameStates;

            _dialogueBehaiviour.OnDialogueStarted -= HideCursorPoint;
            _dialogueBehaiviour.OnDialogueFinished -= ShowCursorPoint;
        }

        private void Start()
        {
            _cursor.enabled = false;
        }

        private void HandleGameStates(GameStates gameStates)
        {
            switch (gameStates)
            {
                case GameStates.WaitForPlayerInput:
                    _pressAnyKeyToStartText.enabled = true;
                    _playerInput.OnKeyPressed += WaitToPressKey;
                    break;
                case GameStates.FadingIntoCutscene:
                    FadeIn(() => _gameManager.ChangeGameState(GameStates.LastCutsceneStarted), _beforeCutSceneFading);
                    break;
                case GameStates.LastCutsceneStarted:
                    _littlelaterText.enabled = true;
                    StartCoroutine(WaitForSeconds());
                    break;
                default:
                    break;
            }
        }

        private void WaitToPressKey(KeyCode keyCode)
        {
            _pressAnyKeyToStartText.enabled = false;
            FadeOut();
            _gameManager.ChangeGameState(GameStates.DemoStarted);
            _playerInput.OnKeyPressed -= WaitToPressKey;
        }

        private void FadeOut()
        {
            StartActionAfterFading(false, OnFadeOutAtStart, DefaultFadeTimeMultiplier);
        }

        public void FadeOut(Action action, float multiplier)
        {
            StartActionAfterFading(false, action, multiplier);
        }

        public void FadeIn(Action action, float multiplier)
        {
            StartActionAfterFading(true, action, multiplier);
        }

        public void StartActionAfterFading(bool fadeIn, Action action, float multiplier)
        {
            StartCoroutine(StartBackgroundFade(fadeIn, multiplier, action, fadeIn ? _fadeInCurve : _fadeOutCurve));
        }

        private IEnumerator StartBackgroundFade(bool fadeIn, float fadeTimeMultiplier, Action action, AnimationCurve curve)
        {
            Color currentColor = _backgroundFadeImage.color;

            for (float i = 0; i < 1f; i += fadeTimeMultiplier * Time.deltaTime)
            {
                currentColor.a = curve.Evaluate(i);
                _backgroundFadeImage.color = currentColor;
                yield return null;
            }

            currentColor.a = curve.Evaluate(1f);
            _backgroundFadeImage.color = currentColor;
            action?.Invoke();
        }

        private void HideCursorPoint()
        {
            _cursor.enabled = false;
            HideInfoPanal();
        }

        private void ShowCursorPoint()
        {
            _cursor.enabled = true;
        }

        public void ShowInfoPanal(IDragable draggableInfo)
        {
            _infoPanalCanvasGroup.alpha = 1f;
            _infoPanalCanvasGroup.blocksRaycasts = true;

            _itemName.text = draggableInfo.GetName();
            //_actionText.text = draggableInfo.GetInteractName();
        }

        public void ShowMoneyEarnedUI(Action action)
        {
            _moneyPopUp.Setup(2.99f);
            action?.Invoke();
        }

        public void HideInfoPanal()
        {
            _infoPanalCanvasGroup.alpha = 0f;
            _infoPanalCanvasGroup.blocksRaycasts = false;
        }


        private IEnumerator WaitForSeconds()
        {
            yield return Helpers.Helper.GetWait(2f);
            _littlelaterText.enabled = false;
            FadeOut(null, 0.3f);
        }

        public void GameOverPanal()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _finishPanal.SetActive(true);
        }
    }
}
