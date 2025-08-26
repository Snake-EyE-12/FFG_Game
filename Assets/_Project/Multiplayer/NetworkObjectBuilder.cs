using Unity.Netcode;
using UnityEngine;

public class NetworkObjectBuilder : INetworkObjectBuilder
{
    private NetworkObject _prefab;
    private Transform _parent;
    private Quaternion _rotation;
    private GameCharacterController _controller;
    private Vector3 _position;
    private bool _destroyWithScene;


    public NetworkObjectBuilder(NetworkObject prefab)
    {
        _prefab = prefab;
    }

    public INetworkObjectBuilder WithSceneDestruction(bool destroyWithScene = true)
    {
        _destroyWithScene = destroyWithScene;
        return this;
    }
    public INetworkObjectBuilder WithPosition(Vector3 pos)
    {
        _position = pos;
        return this;
    }
    public INetworkObjectBuilder WithParent(Transform parent)
    {
        _parent = parent;
        return this;
    }
    public INetworkObjectBuilder WithRotation(Quaternion rot)
    {
        _rotation = rot;
        return this;
    }
    
    public NetworkObject Build()
    {
        NetworkObject no = GameObject.Instantiate(_prefab, _position, _rotation, _parent).GetComponent<NetworkObject>();
        no.Spawn(_destroyWithScene);
        return no;
    }
}

public interface INetworkObjectBuilder
{
    INetworkObjectBuilder WithSceneDestruction(bool destroyWithScene = true);
    INetworkObjectBuilder WithParent(Transform parent);
    INetworkObjectBuilder WithPosition(Vector3 pos);
    INetworkObjectBuilder WithRotation(Quaternion rot);
    NetworkObject Build();

}