using UnityEngine;

public class GameCharacterDummy : MonoBehaviour
{
    private GameCharacterController characterController;
    public void Initialize(GameCharacterController controller)
    {
        // Save controller when needing to send back data ( Collisions )
        characterController = controller;
    }
    
    // ALL INPUT EVENT METHODS CALLED BY CONTROLLER
    
    
    //
}