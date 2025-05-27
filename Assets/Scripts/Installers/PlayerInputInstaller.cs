using UnityEngine;
using Zenject;


namespace ReaperGS
{
    public class PlayerInputInstaller : MonoInstaller
    {
        [SerializeField] private PlayerInput _playerInputInstance;


        public override void InstallBindings()
        {
            Container.Bind<PlayerInput>().FromInstance(_playerInputInstance).AsSingle();
        }
    }
}

