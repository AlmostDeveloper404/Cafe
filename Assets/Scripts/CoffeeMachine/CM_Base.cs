using UnityEngine;

namespace ReaperGS
{
    public abstract class CM_Base<T> where T : CoffeeMachine
    {
        public abstract void EntryState(T coffeeMachine);
        public abstract void UpdateState(T coffeeMachine);
        public abstract void ExitState(T coffeeMachine);
    }
}
