using UnityEngine;

namespace ReaperGS
{
    public class CM_AwaitLid : CM_Base<CoffeeMachine>
    {
        private PlayerInteractions _playerInteractions;
        private CoffeeMachine _coffeeMachine;
        private GameObject _ghostLid;
        private Transform _snapPoint;

        public CM_AwaitLid(PlayerInteractions playerInteractions, CoffeeMachine coffeeMachine, GameObject ghostLid, Transform snapPoint)
        {
            _playerInteractions = playerInteractions;
            _coffeeMachine = coffeeMachine;
            _ghostLid = ghostLid;
            _snapPoint = snapPoint;
        }

        public override void EntryState(CoffeeMachine coffeeMachine)
        {
            IDragable currentDraggable = _playerInteractions.GetCurrentDraggable;
            UpdateVisualHighlighing(currentDraggable);

            _playerInteractions.OnDraggableChanged += UpdateVisualHighlighing;
            _playerInteractions.OnDraggableReleased += TryPlaceLid;
        }

        private void UpdateVisualHighlighing(IDragable dragable)
        {
            _ghostLid.SetActive(dragable != null && dragable as CoffeeLid);
        }

        private void TryPlaceLid(IDragable dragable)
        {
            CoffeeLid coffeeCap = dragable as CoffeeLid;
            if (coffeeCap && coffeeCap.IsInSnapZone)
            {
                coffeeCap.SnapToPoint(_snapPoint, _coffeeMachine.ChangeToLidAttachedState);
                _ghostLid.gameObject.SetActive(false);
            }
        }

        public override void ExitState(CoffeeMachine coffeeMachine)
        {
            _playerInteractions.OnDraggableChanged -= UpdateVisualHighlighing;
            _playerInteractions.OnDraggableReleased -= TryPlaceLid;
        }

        public override void UpdateState(CoffeeMachine coffeeMachine)
        {

        }
    }
}
