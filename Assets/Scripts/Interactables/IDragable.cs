using UnityEngine;

namespace ReaperGS
{
    public interface IDragable
    {
        string GetName();
        string GetInteractName();
        void StartDragging();
        void Throw();
        void Drag(Vector3 targetPos);
        void Release();
        void Highlight();      
        void StopHightlighting();    
    }
}
