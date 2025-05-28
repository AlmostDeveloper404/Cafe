using UnityEngine;
using ReaperGS;
using Zenject;

namespace cherrydev
{
    public class DialogDisplayer : MonoBehaviour
    {
        [Header("MAIN COMPONENT")]
        [SerializeField] private DialogBehaviour _dialogBehaviour;

        [Header("NODE PANELS")]
        [SerializeField] private SentencePanel _dialogSentencePanel;
        [SerializeField] private AnswerPanel _dialogAnswerPanel;

        private void OnEnable()
        {
            _dialogBehaviour.AddListenerToDialogFinishedEvent(DisableDialogPanel);

            _dialogBehaviour.OnAnswerButtonSetUp += SetUpAnswerButtonsClickEvent;
            _dialogBehaviour.OnAnswerNodeSetUp += SetUpAnswerDialogPanel;

            _dialogBehaviour.OnDialogTextCharWrote += _dialogSentencePanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.OnDialogTextSkipped += _dialogSentencePanel.ShowFullDialogText;

            _dialogBehaviour.OnSentenceNodeActive += EnableDialogSentencePanel;
            _dialogBehaviour.OnSentenceNodeActive += DisableDialogAnswerPanel;
            _dialogBehaviour.OnSentenceNodeActive += _dialogSentencePanel.ResetDialogText;
            _dialogBehaviour.OnSentenceNodeActiveWithParameter += _dialogSentencePanel.Setup;

            _dialogBehaviour.OnAnswerNodeActive += EnableDialogAnswerPanel;

            _dialogBehaviour.OnAnswerNodeActiveWithParameter += EnableCertainAmountOfButtonsInAnswerPanal;

            _dialogBehaviour.OnMaxAmountOfAnswerButtonsCalculated += SetUpAnswerButtons;

        }

        private void OnDisable()
        {
            _dialogBehaviour.RemoveListener(DisableDialogPanel);

            _dialogBehaviour.OnAnswerButtonSetUp -= SetUpAnswerButtonsClickEvent;
            _dialogBehaviour.OnAnswerNodeSetUp -= SetUpAnswerDialogPanel;

            _dialogBehaviour.OnDialogTextCharWrote -= _dialogSentencePanel.IncreaseMaxVisibleCharacters;
            _dialogBehaviour.OnDialogTextSkipped -= _dialogSentencePanel.ShowFullDialogText;

            _dialogBehaviour.OnSentenceNodeActive -= EnableDialogSentencePanel;
            _dialogBehaviour.OnSentenceNodeActive -= DisableDialogAnswerPanel;
            _dialogBehaviour.OnSentenceNodeActive -= _dialogSentencePanel.ResetDialogText;
            _dialogBehaviour.OnSentenceNodeActiveWithParameter -= _dialogSentencePanel.Setup;

            _dialogBehaviour.OnAnswerNodeActive -= EnableDialogAnswerPanel;

            _dialogBehaviour.OnAnswerNodeActiveWithParameter -= EnableCertainAmountOfButtonsInAnswerPanal;
            _dialogBehaviour.OnMaxAmountOfAnswerButtonsCalculated -= SetUpAnswerButtons;

        }

        private void EnableCertainAmountOfButtonsInAnswerPanal(int amount)
        {
            _dialogAnswerPanel.EnableCertainAmountOfButtons(amount);
        }

        private void SetUpAnswerButtons(int buttons)
        {
            _dialogAnswerPanel.SetUpButtons(buttons);
        }

        /// <summary>
        /// Disable dialog answer and sentence panel
        /// </summary>
        public void DisableDialogPanel()
        {
            DisableDialogAnswerPanel();
            DisableDialogSentencePanel(null);
        }

        /// <summary>
        /// Enable dialog answer panel
        /// </summary>
        public void EnableDialogAnswerPanel(AnswerNode answerNode)
        {

            ActiveGameObject(_dialogAnswerPanel.gameObject, true);
            _dialogAnswerPanel.DisableAllButtons();
        }

        /// <summary>
        /// Disable dialog answer panel
        /// </summary>
        public void DisableDialogAnswerPanel() =>
            ActiveGameObject(_dialogAnswerPanel.gameObject, false);

        /// <summary>
        /// Enable dialog sentence panel
        /// </summary>
        public void EnableDialogSentencePanel()
        {
            _dialogSentencePanel.ResetDialogText();
            ActiveGameObject(_dialogSentencePanel.gameObject, true);
        }

        /// <summary>
        /// Disable dialog sentence panel
        /// </summary>
        public void DisableDialogSentencePanel(AnswerNode answerNode)
        {
            ActiveGameObject(_dialogSentencePanel.gameObject, false);
        }

        /// <summary>
        /// Enable or disable game object depends on isActive bool flag
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="isActive"></param>
        public void ActiveGameObject(GameObject gameObject, bool isActive)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("Game object is null");
                return;
            }

            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Removing all listeners and Setting up answer button onClick event
        /// </summary>
        /// <param name="index"></param>
        /// <param name="answerNode"></param>
        public void SetUpAnswerButtonsClickEvent(int index, AnswerNode answerNode)
        {
            _dialogAnswerPanel.GetButtonByIndex(index).onClick.RemoveAllListeners();
            _dialogAnswerPanel.AddButtonOnClickListener(index, () =>
            {
                _dialogBehaviour.SetCurrentNodeAndHandleDialogGraph(answerNode.ChildSentenceNodes[index]);
            });
        }

        /// <summary>
        /// Setting up answer dialog panel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="answerText"></param>
        public void SetUpAnswerDialogPanel(int index, string answerText, AnswerNode answerNode)
        {

            _dialogAnswerPanel.GetButtonTextByIndex(index).text = answerText;
        }
    }
}