using cherrydev;
using UnityEngine;
using Zenject;

public class DialogueBehaiviourInstaller : MonoInstaller
{
    [SerializeField] private DialogBehaviour _dialogueBehaiviour;
    public override void InstallBindings()
    {
        Container.Bind<DialogBehaviour>().FromInstance(_dialogueBehaiviour).AsSingle();
    }
}