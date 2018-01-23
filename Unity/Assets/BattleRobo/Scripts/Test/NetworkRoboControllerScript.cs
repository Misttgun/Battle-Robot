using UnityEngine;

public class NetworkRoboControllerScript : Photon.PunBehaviour
{
    [SerializeField]
    private float roboSpeed = 4f;

    [SerializeField]
    private float aimSensitivity = 5f;

    [SerializeField]
    private float aimSensitivityY = 10f;

    [SerializeField]
    private float flyForce = 1000f;

    [SerializeField]
    private float fuelDecreaseSpeed = 0.1f;

    [SerializeField]
    private float fuelRegenSpeed = 0.05f;

    [SerializeField] 
    private float health = 100f;
    
    //[SerializeField]
    //RectTransform fuelFill;

    [SerializeField]
    private Transform roboWheel;

    [SerializeField]
    private Transform roboChest;

    [SerializeField]
    private Rigidbody roboRb;

    [SerializeField]
    private GameObject cam;

    private float fuelAmount = 1f;
    private Vector3 roboVelocity;
    private Vector3 roboRotY;

    private float roboRotUD;
    private float currentRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        if (cam != null)
        {
            if (photonView.isMine)
            {
                cam.SetActive(true);
            }
        }
    }

    private void Update()
    {
        // Mouvemment (Z,Q,S,D)
        if (!photonView.isMine && PhotonNetwork.connected) return;

        // Cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

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

        if (fly != Vector3.zero)
        {
            roboRb.AddForce(fly * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        //fuelFill.localScale = new Vector3(1f, fuelAmount, 1f);
    }

    private void FixedUpdate()
    {
        if (!photonView.isMine && PhotonNetwork.connected) return;

        Vector3 movX = transform.right * Input.GetAxisRaw("Horizontal");
        Vector3 movY = transform.forward * Input.GetAxisRaw("Vertical");

        roboVelocity = (movX + movY).normalized * roboSpeed;

        // Souris (Aim)
        //roboRotY = Vector3.up * Input.GetAxisRaw("Mouse X") * aimSensitivityY;
        roboRotUD = Input.GetAxisRaw("Mouse Y") * aimSensitivity;

        if (roboVelocity != Vector3.zero)
        {
            roboRb.MovePosition(roboRb.position + roboVelocity * Time.deltaTime);
        }

        roboRb.MoveRotation(roboRb.rotation * Quaternion.Euler(0,
                                Input.GetAxisRaw("Mouse X") * aimSensitivityY,
                                0));
        
        //roboWheel.Rotate(roboRotY * Time.deltaTime);
        currentRot -= roboRotUD;
        currentRot = Mathf.Clamp(currentRot, -70f, 70f);
        roboChest.transform.localEulerAngles = new Vector3(0f, 0f, -currentRot);
    }

    public float getFuelAmount()
    {
        return fuelAmount;
    }

    [PunRPC]
    public void TakeDamage(float amount)
    {
        Debug.Log("I AM TOUCH !");
        health -= amount;

        if (health < 0f)
        {
            Debug.Log("I AM DEAD !");
            health = 0f;
            // player is dead
            Die();
        }
    }

    void Die()
    {
        var photonId = GetComponent<PhotonView>().instantiationId;
        
        // classic instanciation
        if (photonId == 0)
            Destroy(gameObject);

        // instantiation over network
        else
        {
            if (GetComponent<PhotonView>().isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}