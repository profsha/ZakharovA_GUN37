using System;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    private Controls _controls;
    
    public override void InstallBindings()
    {
        _controls = new Controls();
        _controls.Game.Enable();
        Container.BindInstance(_controls.Game).AsSingle();
        Container.Bind<BattleController>().AsSingle();
        Container.Bind<PlayerController>().AsSingle();
        Container.Bind<SceneController>().AsSingle();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }
}