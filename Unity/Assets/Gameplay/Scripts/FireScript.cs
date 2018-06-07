using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class FireScript : MonoBehaviour {

    [SerializeField]
    private Camera camFPS;

    [SerializeField]
    private LayerMask mask;

    [SerializeField] public Image hitMarker;
    	
    [SerializeField]private Transform bulletSpawn; 
    
    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private float bulletSpeed;
    
    public GunScript gun;
    private Vector3 hitPoint;
    GameObject spawnBullet;
    private void Update()
    {

        if(Input.GetButtonDown("Fire1"))
        { 
            fire();  
            bulletPropagation();
        }
        if(spawnBullet)
        {
            Vector3 hitDir = hitPoint - bulletSpawn.position; //direction le zone touché par le raycast 
            spawnBullet.transform.Translate(hitDir.normalized*bulletSpeed*Time.deltaTime, Space.World); 
            
        }
        
    }

    private void fire()
    {
        RaycastHit shot;

        if(Physics.Raycast(camFPS.transform.position, camFPS.transform.forward , out shot, gun.range, mask))
        {
            
            Debug.Log(" hit " + shot.collider.name);
            
            //zone touché
            hitPoint = shot.point; 
            StopCoroutine("displayHitMarker");      
            hitMarker.enabled=true;                //afficher le hit marker sur le UI 
            StartCoroutine("displayHitMarker");
        }
        else
        {
            hitPoint = camFPS.transform.position + (camFPS.transform.forward * gun.range);
        }
    }
    
    private void bulletPropagation()
    {  
        spawnBullet = Instantiate(bullet, bulletSpawn.position, camFPS.transform.rotation);
        Destroy(spawnBullet, 1.2f);
        
    }
    
    private IEnumerator displayHitMarker()
    {
        yield return new WaitForSeconds(0.15f);
        hitMarker.enabled=false;
    }
    
}
