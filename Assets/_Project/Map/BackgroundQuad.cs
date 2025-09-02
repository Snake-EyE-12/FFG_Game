using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundQuad : MonoBehaviour
{
	public Material backgroundMaterial;
	public float size = 50f;
	public float parallax = 0.1f;

	private GameObject quad;
	private Renderer quadRenderer;

	void Start()
	{
		quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		quad.name = "BackgroundQuad";
		quad.transform.SetParent(transform);
		quad.transform.localPosition = new Vector3(0, 0, 200);
		quad.transform.localRotation = Quaternion.identity;
		quad.transform.localScale = new Vector3(size, size, 1);

		quadRenderer = quad.GetComponent<Renderer>();
		quadRenderer.material = backgroundMaterial;

		quad.layer = LayerMask.NameToLayer("Ignore Raycast"); 
	}

	void Update()
	{
		if (quadRenderer == null) return;

		Vector3 camPos = transform.position;
		Vector2 offset = new Vector2(camPos.x, camPos.z) * parallax;
		quadRenderer.material.mainTextureOffset = offset;
	}
}
