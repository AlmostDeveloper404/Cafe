using ReaperGS;
using UnityEngine;
using Zenject;

public class FPSCameraControllerInstaller : MonoInstaller
{
    [SerializeField] private FPSCameraController _fpsCameraController;
    public override void InstallBindings()
    {
        Container.Bind<FPSCameraController>().FromInstance(_fpsCameraController).AsSingle();
    }
}