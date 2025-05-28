using UnityEngine;

namespace ReaperGS
{
    public class CM_DefaultState : CM_Base<CoffeeMachine>
    {
        private CoffeeMachine _coffeeMachine;
        private PlayerInteractions _playerInteractions;
        private GameObject _coffeeGhostCup;
        private Transform _snapPoint;

        public CM_DefaultState(PlayerInteractions playerInteractions, CoffeeMachine coffeeMachine, GameObject ghostCup, Transform snapPoint)
        {
            _playerInteractions = playerInteractions;
            _coffeeMachine = coffeeMachine;
            _coffeeGhostCup = ghostCup;
            _snapPoint = snapPoint;
        }

        public override void EntryState(CoffeeMachine coffeeMachine)
        {
            _playerInteractions.OnDraggableChanged += WaitForCap;
            _playerInteractions.OnDraggableReleased += TryPlaceDraggable;
        }

        private void WaitForCap(IDragable dragable)
        {
            _coffeeGhostCup.SetActive(dragable != null && dragable as EmptyCup);
        }

        private void TryPlaceDraggable(IDragable dragable)
        {
            EmptyCup coffeeCap = dragable as EmptyCup;
            if (coffeeCap && coffeeCap.IsInSnapZone)
            {
                coffeeCap.SnapToPoint(_snapPoint, _coffeeMachine.ChangeToCupInsertedState);
                _coffeeGhostCup.gameObject.SetActive(false);
            }
        }

        public override void UpdateState(CoffeeMachine coffeeMachine)
        {

        }

        public override void ExitState(CoffeeMachine coffeeMachine)
        {
            _playerInteractions.OnDraggableChanged -= WaitForCap;
            _playerInteractions.OnDraggableReleased -= TryPlaceDraggable;
        }

    }
}
