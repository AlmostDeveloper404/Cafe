using UnityEngine;
using Zenject;

namespace ReaperGS
{
    public class CoffeeCup : DraggableObject
    {
        

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }	
    }
}
