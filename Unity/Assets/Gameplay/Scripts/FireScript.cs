using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour {

    [SerializeField]
    private Camera camFPS;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private GameObject bulletSpawn; 

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private float bulletSpeed;

    public GunScript gun;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            bulletPropagation();
            fire();
        }
    }
    private void fire()
    {
        RaycastHit shot;
        if(Physics.Raycast(camFPS.transform.position, camFPS.transform.forward , out shot, gun.range, mask))
        {
            Debug.LogError(" hit " + shot.collider.name);
            shot.rigidbody.AddForce(camFPS.transform.forward * bulletSpeed/100);
        }

    }
    private void bulletPropagation()
    {
        GameObject spawnBullet;
        spawnBullet = Instantiate(bullet, bulletSpawn.transform.position, camFPS.transform.rotation);

        Rigidbody bulletRgb;
        bulletRgb = spawnBullet.GetComponent<Rigidbody>();

        bulletRgb.AddForce(camFPS.transform.forward * bulletSpeed);

        Destroy(spawnBullet, 3.0f);
    }
}
