using UnityEngine;
using Zenject;
using System.Collections;
using System;
using Helpers;

namespace ReaperGS
{
    public class CoffeeMachine : MonoBehaviour
    {
        private PlayerInteractions _playerInteractions;

        [SerializeField] private Animator _visualCupAnimator;
        [SerializeField] private GameObject _readyCoffeePref;

        [SerializeField] private Transform _placePoint;
        [SerializeField] private ParticleSystem _coffeeStreamParticles;

        [Header("Highlighing")]
        [SerializeField] private GameObject _coffeeCupGhost;
        [SerializeField] private GameObject _lidGhost;

        private CM_Base<CoffeeMachine> _currentState;
        private CM_DefaultState _defaultState;
        private CM_CupInserted _cupInsertedState;
        private CM_AwaitLid _awaitLidState;
        private CM_LidAttached _lidAttachedState;

        private Coroutine _waitForSecondsCoroutine;




        [Inject]
        private void Construct(FPSCameraController cameraController)
        {
            _playerInteractions = cameraController.GetComponent<PlayerInteractions>();
        }

        private void Start()
        {
            _coffeeCupGhost.SetActive(false);
            _lidGhost.SetActive(false);

            _defaultState = new CM_DefaultState(_playerInteractions, this, _coffeeCupGhost, _placePoint);
            _awaitLidState = new CM_AwaitLid(_playerInteractions, this, _lidGhost, _placePoint);
            _lidAttachedState = new CM_LidAttached(_readyCoffeePref, _placePoint);
            _cupInsertedState = new CM_CupInserted(_visualCupAnimator, _coffeeStreamParticles);

            _currentState = _defaultState;
            _currentState?.EntryState(this);
        }

        private void Update()
        {
            _currentState?.UpdateState(this);
        }

        private void ChangeState(CM_Base<CoffeeMachine> newState)
        {
            _currentState?.ExitState(this);
            _currentState = newState;
            _currentState?.EntryState(this);
        }

        public void ChangeToCupInsertedState()
        {
            _visualCupAnimator.gameObject.SetActive(true);
            ChangeState(_cupInsertedState);
        }

        public void ChangeToLidAttachedState()
        {

            ChangeState(_lidAttachedState);
        }

        public void ChangeToAwaitLidState()
        {
            ChangeState(_awaitLidState);
        }

        public void ChangeToDefault()
        {
            _visualCupAnimator.gameObject.SetActive(false);
            ChangeState(_defaultState);
        }

        public void WaitForSecondsWithAction(float timeToWait, Action action)
        {
            if (_waitForSecondsCoroutine != null)
            {
                StopCoroutine(_waitForSecondsCoroutine);
            }

            _waitForSecondsCoroutine = StartCoroutine(WaitToInvokeAction(timeToWait, action));
        }

        private IEnumerator WaitToInvokeAction(float timeToWait, Action action)
        {
            yield return Helper.GetWait(timeToWait);
            action?.Invoke();
        }
    }
}
