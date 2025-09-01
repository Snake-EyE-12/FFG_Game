using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    void LateUpdate()
    {
		mainCam = Camera.main;
		transform.forward = mainCam.transform.forward;
    }
}