using UnityEngine;



public class JumpPad : MonoBehaviour
{
    public float jumpForce = 10f;
    public PlayerMovement playerMovement;
    public Collider playercollider;

    private void OnTriggerEnter(Collider playercollider)
    {
        // This code needs to be inside the OnTriggerEnter method
        if (playercollider.CompareTag("Player")) // Ensure your player GameObject has the "Player" tag
        {
        if (playerMovement != null)
        {
            // Call the method to make the player jump
            playerMovement.InitiateJumpPadJump(jumpForce);
        }
    }
}

}


