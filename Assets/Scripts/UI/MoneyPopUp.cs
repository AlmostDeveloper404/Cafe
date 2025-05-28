using TMPro;
using UnityEngine;

namespace ReaperGS
{
    public class MoneyPopUp : MonoBehaviour
    {
        private Animator _animator;

        [SerializeField] private TMP_Text _moneyAmount;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Setup(float amount)
        {
            _moneyAmount.text = $"+{amount}$";
            _animator.SetTrigger(Animations.MoneyEarned);
        }
    }
}
