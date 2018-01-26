using System;
using UnityEngine;

public class RoboMotor : MonoBehaviour
{
    [SerializeField]
    private Transform roboWheel;
    [SerializeField]
    private Transform roboChest;
    [SerializeField]
    private Rigidbody roboRb;
    [SerializeField]
    private Camera cam;

   
    private Vector3 roboVelocity = Vector3.zero;
    private Vector3 roboRotY = Vector3.zero;
    private Vector3 flyForce = Vector3.zero;
    private float roboRotUD = 0f;
    private float currentRot = 0f;
    private bool cursorLocked;


    public void roboMove(Vector3 velocity)
    {
        
        roboVelocity = velocity;
    }
    public void roboRotate(Vector3 rotY, float rotUD)
    {
        roboRotY = rotY;
        roboRotUD = rotUD;
    }
    public void roboJump(Vector3 fly)
    {
        flyForce = fly; 
    }

	private void FixedUpdate ()
    {
        
        if (roboVelocity != Vector3.zero)
        {
            roboWheel.position += roboWheel.rotation * roboVelocity * Time.deltaTime;
        }
        roboWheel.Rotate(roboRotY * Time.deltaTime);
        currentRot -= roboRotUD;
        currentRot = Mathf.Clamp(currentRot, -70, 70) ;
        roboChest.transform.localEulerAngles = new Vector3(0f,0f, -currentRot);
        if(flyForce!=Vector3.zero)
        {
            roboRb.AddForce(flyForce*Time.fixedDeltaTime  , ForceMode.Impulse);
        }
        
	}

    public void toggleCursorState()
    {
        cursorLocked = !cursorLocked;
    }


    public void lockOrUnlockCursor()
    {
        if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
         
     
}
