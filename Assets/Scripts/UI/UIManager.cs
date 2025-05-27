using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using Zenject;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

namespace ReaperGS
{
    public class UIManager : MonoBehaviour
    {
        public event Action OnFadeOutAtStart;

        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _actionText;
        [SerializeField] private CanvasGroup _infoPanalCanvasGroup;

        [Header("FadingImage")]
        [field: SerializeField] public float DefaultFadeTimeMultiplier { get; private set; }

        [SerializeField] private Image _backgroundFadeImage;
        [SerializeField] private AnimationCurve _fadeOutCurve;
        [SerializeField] private AnimationCurve _fadeInCurve;

        private PlayerInteractions _playerInteractions;

        [Inject]
        private void Construct(FPSCameraController fPSCameraController)
        {
            _playerInteractions = fPSCameraController.GetComponent<PlayerInteractions>();
        }

        private void OnEnable()
        {
            _playerInteractions.OnInteractableOver += ShowInfoPanal;
            _playerInteractions.OnInteractionReset += HideInfoPanal;
        }

        private void OnDisable()
        {
            _playerInteractions.OnInteractableOver -= ShowInfoPanal;
            _playerInteractions.OnInteractionReset -= HideInfoPanal;
        }

        private void Start()
        {
            FadeOut();
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

        public void ShowInfoPanal(IDragable draggableInfo)
        {
            _infoPanalCanvasGroup.alpha = 1f;
            _infoPanalCanvasGroup.blocksRaycasts = true;

            _itemName.text = draggableInfo.GetName();
            //_actionText.text = draggableInfo.GetInteractName();
        }

        public void HideInfoPanal()
        {
            _infoPanalCanvasGroup.alpha = 0f;
            _infoPanalCanvasGroup.blocksRaycasts = false;
        }

    }
}
