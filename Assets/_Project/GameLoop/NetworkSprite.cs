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


	private NetworkVariable<bool> flipX = new NetworkVariable<bool>(
		false,
		NetworkVariableReadPermission.Everyone,
		NetworkVariableWritePermission.Server);

	public int SpriteIndex => spriteIndex.Value;
	public Sprite CurrentSprite =>
		spriteIndex.Value >= 0 && spriteIndex.Value < availableSprites.Length
			? availableSprites[spriteIndex.Value]
			: null;

	public bool FlipX => flipX.Value;


	private void Awake()
	{
		if (spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public override void OnNetworkSpawn()
	{
		spriteIndex.OnValueChanged += OnSpriteChanged;
		flipX.OnValueChanged += OnFlipChanged;

		OnSpriteChanged(0, spriteIndex.Value);
		OnFlipChanged(false, flipX.Value);
	}

	private void OnDestroy()
	{
		spriteIndex.OnValueChanged -= OnSpriteChanged;
		flipX.OnValueChanged -= OnFlipChanged;
	}

	private void OnSpriteChanged(int oldValue, int newValue)
	{
		if (newValue >= 0 && newValue < availableSprites.Length)
		{
			spriteRenderer.sprite = availableSprites[newValue];
		}
	}


	private void OnFlipChanged(bool oldValue, bool newValue)
	{
		spriteRenderer.flipX = newValue;
	}

	[ServerRpc]
	public void ChangeSpriteServerRpc(int newIndex)
	{
		if (newIndex >= 0 && newIndex < availableSprites.Length)
		{
			spriteIndex.Value = newIndex;
		}
	}

	[ServerRpc]
	public void SetFlipXServerRpc(bool newFlipX)
	{
		if(newFlipX != flipX.Value) flipX.Value = newFlipX;
	}
}
