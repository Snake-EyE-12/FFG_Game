using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class BindableObject<TController, TBindable, TBrain> : MonoBehaviour
    where TController : BindingController<TController, TBindable, TBrain>
    where TBindable : BindableObject<TController, TBindable, TBrain>
    where TBrain : PlayerBrain<TController, TBindable, TBrain>
{
    protected TBrain bindedBrain;
    public void Initialize(TBrain binding)
    {
        bindedBrain = binding;
    }
}




public abstract class BindingController<TController, TBindable, TBrain> : MonoBehaviour 
    where TController : BindingController<TController, TBindable, TBrain>
    where TBindable : BindableObject<TController, TBindable, TBrain>
    where TBrain : PlayerBrain<TController, TBindable, TBrain>
{
    [SerializeField] protected TBindable prefab;
    [SerializeField] protected Transform parent;
    public virtual TBindable CreateBinding()
    {
        return Instantiate(prefab, parent);
    }
}


public abstract class PlayerBrain<TController, TBindable, TBrain> : NetworkBehaviour
    where TController : BindingController<TController, TBindable, TBrain>
    where TBindable : BindableObject<TController, TBindable, TBrain>
    where TBrain : PlayerBrain<TController, TBindable, TBrain>
{
    protected TBindable binding;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        PrepareBrain();
    }

    private void PrepareBrain()
    {
        TController attachmentAttempt = FindFirstObjectByType<TController>();
        if (attachmentAttempt == null) return;
        binding = attachmentAttempt.CreateBinding();
        binding.Initialize((TBrain)this);
        OnInitialized();
    }
    private void OnSceneLoaded(string a, LoadSceneMode b, List<ulong> c, List<ulong> d)
    {
        PrepareBrain();
    }

    protected abstract void OnInitialized();
}