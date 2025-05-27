using UnityEngine;

namespace ReaperGS
{
    [CreateAssetMenu(menuName = "ReaperGS/Draggable/Create", fileName = "New Draggable")]
    public class DraggableSO : ScriptableObject
    {
        public string Name;
        public string Action;
    }
}
