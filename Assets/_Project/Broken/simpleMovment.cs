using UnityEngine;
using UnityEngine.InputSystem;

public class simpleMovment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.wKey.isPressed) transform.Translate(Vector3.forward * Time.deltaTime);
        if(Keyboard.current.sKey.isPressed) transform.Translate(Vector3.back * Time.deltaTime);
        if(Keyboard.current.dKey.isPressed) transform.Translate(Vector3.right * Time.deltaTime);
        if(Keyboard.current.aKey.isPressed) transform.Translate(Vector3.left * Time.deltaTime);
    }
}
