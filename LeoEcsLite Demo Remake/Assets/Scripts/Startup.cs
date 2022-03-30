using Systems;
using Systems.PointAndClickMovement;
using Components;
using Events;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Services;
using UnityEngine;
using Voody.UniLeo.Lite;

public class Startup : MonoBehaviour
{
    private EcsSystems _updateSystems;
    private EcsSystems _fixedUpdateSystems;

    private void Awake()
    {
        var ecsWorld = new EcsWorld();
        SetInitSystems(ecsWorld);
        SetUpdateSystems(ecsWorld);
        SetFixedUpdateSystems(ecsWorld);
    }

    private void SetInitSystems(EcsWorld ecsWorld)
    {
        var initSystems = new EcsSystems(ecsWorld)
            .ConvertScene()
            .Add(new ListenersRegisterSystem())
            .Add(new PlayerInputInitSystem())
            // .Add(new PointAndClickMovementEndMarkerInitSystem())
            .Inject(new UnityViewService());
        initSystems.Init();
    }

    private void SetUpdateSystems(EcsWorld ecsWorld)
    {
        _updateSystems = new EcsSystems(ecsWorld)
            .DelHere<PlayerStartMovingEvent>()
            .DelHere<MouseHitComponent>()
            .Add(new PlayerInputUpdateSystem())
            .Add(new MousePositionCheckSystem())
            .Add(new PointAndClickMovementStartCheckSystem())
            .Add(new PlayerAnimationUpdateSystem())
            // .Add(new PointAndClickMovementEndMarkerUpdateSystem())
            .Inject(new UnityInputService())
            .Inject(new UnityPhysicsService());
        _updateSystems.Init();
    }

    private void SetFixedUpdateSystems(EcsWorld ecsWorld)
    {
        _fixedUpdateSystems = new EcsSystems(ecsWorld)
            .DelHere<PlayerFinishMovingEvent>()
            .DelHere<ObstacleHitEvent>()
            // .DelHere<PlayerGroundHitEvent>()
            .Add(new PointAndClickMovementPositioningSystem())
            .Add(new PointAndClickMovementDirectingSystem())
            .Add(new ObstacleCheckSystem())
            .Add(new PointAndClickMovementFinishCheckSystem())
            // .Add(new PlayerGroundCheckSystem())
            // .Add(new PlayerDoorOperatorCheckSystem())
            // .Add(new DoorOperatorsWorkingSystem())
            // .Add(new DoorsOperatingSystem())
            .Inject(new UnityTimeService())
            .Inject(new UnityPhysicsService());
        _fixedUpdateSystems.Init();
    }

    private void Update() => _updateSystems.Run();

    private void FixedUpdate() => _fixedUpdateSystems.Run();
}
