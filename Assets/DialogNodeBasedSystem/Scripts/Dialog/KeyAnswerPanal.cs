using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReaperGS
{
    public class KeyAnswerPanal : MonoBehaviour
    {
        [SerializeField] private Button[] _buttons;
        [SerializeField] private TextMeshProUGUI[] _buttonsAnswerText;

        public void AddButtonOnClickListener(int index, UnityAction action) => _buttons[index].onClick.AddListener(action);

        public Button GetButtonByIndex(int index) => _buttons[index];
        public TextMeshProUGUI GetButtonTextByIndex(int index) => _buttonsAnswerText[index];
    }
}
