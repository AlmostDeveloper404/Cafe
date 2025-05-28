using System;
using UnityEngine;

namespace ReaperGS
{
    public enum GameStates
    {
        WaitForPlayerInput, DemoStarted, GameplayStared, LastCutsceneStarted
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
    }
}
