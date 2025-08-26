using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCharacterController : NetworkBehaviour
{
    [SerializeField] private GameCharacterDummy gameCharacterPrefab;

    private GameCharacterDummy dummy;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        
    }
    
    //public void ReceiveInput(Vector2 inputDirection)
    //{
    //    dummy.GetMovement().Move(inputDirection);
    //}

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner) dummy.GetMovement().Move(context.ReadValue<Vector2>());
    }

    public void StartGame()
    {
        Vector3 pos = Spawning.GetSpawnPoint().position;
        dummy = new GameCharacterBuilder(gameCharacterPrefab)
            .WithInitialization(this)
            .WithPosition(pos)
            .Build();

        dummy.GetComponent<NetworkObject>().Spawn();
    }

    private void OnEnable()
    {
        GameStarter.OnGameStart += StartGame;
    }

    private void OnDisable()
    {
        GameStarter.OnGameStart -= StartGame;
    }
}





public class GameCharacterBuilder : IGameCharacterBuilder
{
    private GameCharacterDummy _lobbyUIPrefab;
    private Transform _parent;
    private GameCharacterController _controller;
    private Vector3 _position;


    public GameCharacterBuilder(GameCharacterDummy prefab)
    {
        _lobbyUIPrefab = prefab;
    }

    public IGameCharacterBuilder WithPosition(Vector3 pos)
    {
        _position = pos;
        return this;
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
        GameCharacterDummy gme = GameObject.Instantiate(_lobbyUIPrefab, _position, Quaternion.identity, _parent);
        gme.Initialize(_controller);
        return gme;
    }
}

public interface IGameCharacterBuilder
{
    IGameCharacterBuilder WithParent(Transform parent);
    IGameCharacterBuilder WithPosition(Vector3 pos);
    IGameCharacterBuilder WithInitialization(GameCharacterController controller);
    GameCharacterDummy Build();

}