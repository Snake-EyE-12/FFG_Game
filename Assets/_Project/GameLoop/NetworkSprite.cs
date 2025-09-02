using Unity.Netcode;
using UnityEngine;

public class NetworkSprite : NetworkBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite[] availableSprites;

	private NetworkVariable<int> spriteIndex = new NetworkVariable<int>(
		0,
		NetworkVariableReadPermission.Everyone,
		NetworkVariableWritePermission.Server);

	public int SpriteIndex => spriteIndex.Value;
	public Sprite CurrentSprite =>
		spriteIndex.Value >= 0 && spriteIndex.Value < availableSprites.Length
			? availableSprites[spriteIndex.Value]
			: null;

	private void Awake()
	{
		if (spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public override void OnNetworkSpawn()
	{
		spriteIndex.OnValueChanged += OnSpriteChanged;
		OnSpriteChanged(0, spriteIndex.Value);
	}

	private void OnDestroy()
	{
		spriteIndex.OnValueChanged -= OnSpriteChanged;
	}

	private void OnSpriteChanged(int oldValue, int newValue)
	{
		if (newValue >= 0 && newValue < availableSprites.Length)
		{
			spriteRenderer.sprite = availableSprites[newValue];
		}
	}

	[ServerRpc]
	public void ChangeSpriteServerRpc(int newIndex)
	{
		if (newIndex >= 0 && newIndex < availableSprites.Length)
		{
			spriteIndex.Value = newIndex;
		}
	}
}
