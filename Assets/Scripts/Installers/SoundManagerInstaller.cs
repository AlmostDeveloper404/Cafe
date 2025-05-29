using ReaperGS;
using UnityEngine;
using Zenject;

public class SoundManagerInstaller : MonoInstaller
{
    [SerializeField] private SoundManager _soundManager;

    public override void InstallBindings()
    {
        Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle();
    }
}