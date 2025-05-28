using ReaperGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace cherrydev
{
    public class SentencePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dialogText;

        private string _currentText;

        /// <summary>
        /// Setting dialogText max visible characters to zero
        /// </summary>
        public void ResetDialogText() => _dialogText.maxVisibleCharacters = 0;

        /// <summary>
        /// Set dialog text max visible characters to dialog text length
        /// </summary>
        /// <param name="text"></param>
        public void ShowFullDialogText() => _dialogText.maxVisibleCharacters = _currentText.Length;

        /// <summary>
        /// Increasing max visible characters
        /// </summary>
        /// 

        [ContextMenu("Increase Char")]
        public void IncreaseMaxVisibleCharacters()
        {
            _dialogText.maxVisibleCharacters++;
        }

        /// <summary>
        /// Assigning dialog name text, character image sprite and dialog text
        /// </summary>
        public void Setup(string text)
        {
            //IncreaseMaxVisibleCharacters();
            //IncreaseMaxVisibleCharacters();

            _currentText = $"{text}";
            _dialogText.text = _currentText;
        }
    }
}