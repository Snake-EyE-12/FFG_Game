using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIWobble : MonoBehaviour
{
	[SerializeField] private float amplitude = 10f;
	[SerializeField] private float frequency = 1f;
	[SerializeField] private float noiseScale = 1f;

	private RectTransform rectTransform;
	private Vector3 startPos;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		startPos = rectTransform.anchoredPosition;
	}

	private void Update()
	{
		float t = Time.time * frequency;

		float x = (Mathf.PerlinNoise(t * noiseScale, 0f) - 0.5f) * 2f * amplitude;
		float y = (Mathf.PerlinNoise(0f, t * noiseScale) - 0.5f) * 2f * amplitude;

		rectTransform.anchoredPosition = startPos + new Vector3(x, y, 0f);
	}
}
