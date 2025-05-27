using UnityEngine;

namespace ReaperGS
{
    public class CM_LidAttached : CM_Base<CoffeeMachine>
    {
        private GameObject _completeCoffeePref;
        private Transform _placePoint;

        public CM_LidAttached(GameObject completeCoffeePref, Transform placePoint)
        {
            _completeCoffeePref = completeCoffeePref;
            _placePoint = placePoint;
        }

        public override void EntryState(CoffeeMachine coffeeMachine)
        {    
            GameObject coffeeCup = GameObject.Instantiate(_completeCoffeePref, _placePoint.position, _placePoint.rotation, null);
            coffeeMachine.ChangeToDefault();
        }

        public override void ExitState(CoffeeMachine coffeeMachine)
        {

        }

        public override void UpdateState(CoffeeMachine coffeeMachine)
        {

        }
    }
}
