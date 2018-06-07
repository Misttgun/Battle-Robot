using System.Collections;
using UnityEngine;

using UnityEngine.UI;

public class FireScriptLR : MonoBehaviour {

    [SerializeField]
    private Camera camFPS;

    [SerializeField]
    private LayerMask mask;

    [SerializeField] public Image hitMarker;
    	
    [SerializeField]private Transform bulletSpawn; 
   
    [SerializeField] private LineRenderer laserLine; // faire une ligne à partir de deux points A et B
    
    public GunScript gun;
    private Vector3 hitPoint;
    
    private void Update()
    {

        if(Input.GetButtonDown("Fire1"))
        { 
            fire();
        }
        
    }

    private void fire()
    {
        RaycastHit shot;
        
        // Position du point A
        laserLine.SetPosition(0,bulletSpawn.position);
        
        if(Physics.Raycast(camFPS.transform.position, camFPS.transform.forward , out shot, gun.range, mask))
        {
            
            Debug.Log(" hit " + shot.collider.name);
            hitPoint = shot.point; 
            StopCoroutine("displayHitMarker");
            
            // Position du point B 
            laserLine.SetPosition(1,shot.point); 
            
            hitMarker.enabled=true;
            StartCoroutine("displayHitMarker");
        }
        else
        {
            StopCoroutine("displayHitMarker");
            
            // Position du point B 
            laserLine.SetPosition(1,camFPS.transform.position+(camFPS.transform.forward * gun.range));
            
            StartCoroutine("displayHitMarker");
        }
    }
 
    
    private IEnumerator displayHitMarker()
    {
        // on trace ensuite la ligne entre le point A et B pendant un intervalle de temps 
        laserLine.enabled = true;
        yield return new WaitForSeconds(0.15f);
        hitMarker.enabled=false;
        laserLine.enabled = false;
    }
    
}
