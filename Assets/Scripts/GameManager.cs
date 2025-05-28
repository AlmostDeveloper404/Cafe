using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReaperGS
{
    public enum GameStates
    {
        WaitForPlayerInput, DemoStarted, FadingIntoCutscene, LastCutsceneStarted
    }

    public class GameManager : MonoBehaviour
    {
        public event Action<GameStates> OnNewGameStateEntered;

        private void Start()
        {
            ChangeGameState(GameStates.WaitForPlayerInput);
        }

        public void ChangeGameState(GameStates gameStates)
        {
            OnNewGameStateEntered?.Invoke(gameStates);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
