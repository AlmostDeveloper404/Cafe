using ReaperGS;
using UnityEngine;
using Zenject;

public class UIManagerInstaller : MonoInstaller
{
    [SerializeField] private UIManager _uiManager;

    public override void InstallBindings()
    {
        Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle();
    }
}