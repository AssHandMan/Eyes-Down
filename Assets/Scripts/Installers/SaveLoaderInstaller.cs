using EyesDown.Core;
using UnityEngine;
using Zenject;

public class SaveLoaderInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallSaveLoadManager();
    }

    public void InstallSaveLoadManager()
    {
        Container
            .Bind<SaveLoaderManager>()
            .AsSingle()
            .NonLazy();
    }
}