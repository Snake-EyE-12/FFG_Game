using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISineMover : MonoBehaviour
{
	[SerializeField] private Vector2 direction = Vector2.right; // Line of movement
	[SerializeField] private float amplitude = 50f;             // Distance from center
	[SerializeField] private float frequency = 1f;              // Oscillations per second
	[SerializeField] private float cycleTime = 2f;              // Seconds to complete one back+forth

	private RectTransform rectTransform;
	private Vector2 startPos;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		startPos = rectTransform.anchoredPosition;
		direction.Normalize();
	}

	private void Update()
	{
		// Scale time so a full sine cycle always takes `cycleTime`
		float scaledTime = (Time.time / cycleTime) * Mathf.PI * 2f;

		// Apply frequency as a multiplier without breaking cycle time
		float offset = Mathf.Sin(scaledTime * frequency) * amplitude;

		rectTransform.anchoredPosition = startPos + direction * offset;
	}
}
