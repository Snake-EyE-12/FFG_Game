using Unity.Netcode;
using UnityEngine;

public class GameCharacterController : NetworkBehaviour
{
    [SerializeField] private GameCharacterDummy gameCharacterPrefab;

    private GameCharacterDummy dummy;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        dummy = new GameCharacterBuilder(gameCharacterPrefab)
            .WithInitialization(this)
            .Build();
    }
    
    public void ReceiveInput(Vector2 inputDirection)
    {
        dummy.GetMovement().Move(inputDirection);
    }

}





public class GameCharacterBuilder : IGameCharacterBuilder
{
    private GameCharacterDummy _lobbyUIPrefab;
    private Transform _parent;
    private GameCharacterController _controller;


    public GameCharacterBuilder(GameCharacterDummy prefab)
    {
        _lobbyUIPrefab = prefab;
    }

    public IGameCharacterBuilder WithParent(Transform parent)
    {
        _parent = parent;
        return this;
    }

    public IGameCharacterBuilder WithInitialization(GameCharacterController controller)
    {
        _controller = controller;
        return this;
    }

    public GameCharacterDummy Build()
    {
        GameCharacterDummy gme = GameObject.Instantiate(_lobbyUIPrefab, _parent);
        gme.Initialize(_controller);
        return gme;
    }
}

public interface IGameCharacterBuilder
{
    IGameCharacterBuilder WithParent(Transform parent);
    IGameCharacterBuilder WithInitialization(GameCharacterController controller);
    GameCharacterDummy Build();

}