using UnityEngine;

public class NetworkRoboControllerScript : Photon.PunBehaviour {

	[SerializeField]
    private float roboSpeed = 4f;
    [SerializeField]
    private float aimSensitivity = 5f;
    [SerializeField]
    private float aimSensitivityY =10f;
    [SerializeField]
    private float flyForce = 1000f;
    [SerializeField]
    private float fuelDecreaseSpeed = 0.1f;
    [SerializeField]
    private float fuelRegenSpeed = 0.05f;
    /*[SerializeField]
    RectTransform fuelFill;*/

    private NetworkRoboMotorScript motor;
    private float fuelAmount = 1f;

    private void Start()
    {
        motor = GetComponent < NetworkRoboMotorScript>();
        motor.toggleCursorState();
    }
    private void Update()
    {

        // Mouvemment (Z,Q,S,D)
        if (photonView.isMine)
        {
            float xMov = Input.GetAxisRaw("Horizontal");
            float yMov = Input.GetAxisRaw("Vertical");

            Vector3 movX = transform.right * xMov;
            Vector3 movY = transform.forward * yMov;

            Vector3 roboVelocity = (movX + movY).normalized * roboSpeed;

            motor.roboMove(roboVelocity);

            // Souris (Aim)

            float yRot = Input.GetAxisRaw("Mouse X");
            float rot = Input.GetAxisRaw("Mouse Y");

            Vector3 rotY = Vector3.up * yRot * aimSensitivityY * aimSensitivity;
            float rotUpDown = (rot) * aimSensitivity;

            motor.roboRotate(rotY, rotUpDown);

            //Fly
            Vector3 fly = Vector3.zero;
            if (Input.GetButton("Jump") && fuelAmount > 0f)
            {
                fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
                if (fuelAmount >= 0.04f)
                {
                    fly = Vector3.up * flyForce;
                }

            }
            else
            {
                fuelAmount += fuelRegenSpeed * Time.deltaTime;
            }

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
            motor.roboJump(fly);

            if (Input.GetKeyDown(KeyCode.L))
            {
                motor.toggleCursorState();
            }

            motor.lockOrUnlockCursor();
        }

        //fuelFill.localScale = new Vector3(1f, fuelAmount, 1f);
    }

    public float getFuelAmount()
    {
        return fuelAmount;
    }
}
