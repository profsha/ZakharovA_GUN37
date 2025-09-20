using System.Linq;
using Controllers.Commands;
using GameState;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public CellPalleteSettings CellPalleteSettings;
    private Controls _controls;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<GameEvent>();
        Container.DeclareSignal<GameStatus>();
        
        _controls = new Controls();
        _controls.Game.Enable();
        Container.BindInstance(_controls.Game).AsSingle();

        var units = FindObjectsByType<Unit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        Container.BindInstance(units).AsSingle();
        
        Container.Bind<IGameState>().To<SingleGameState>().AsSingle();
        Container.Bind<IGameplayCommand>().To<CheckerCommand>().AsSingle();
        
        Container.Bind<CellPalleteSettings>().FromInstance(CellPalleteSettings).AsSingle();
        
        Container.Bind<Battlefield>().AsSingle().NonLazy();
        Container.Bind<BattleController>().AsSingle().NonLazy();
        Container.Bind<PlayerController>().AsSingle().NonLazy();
        Container.Bind<SceneController>().AsSingle();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }
}