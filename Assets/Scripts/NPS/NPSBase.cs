using UnityEngine;

namespace ReaperGS
{
    public abstract class NPSBase<T> where T : NPS
    {
        public abstract void EntryState(T nps);
        public abstract void UpdateState(T nps);

        public abstract void ExitState(T nps);
    }
}
