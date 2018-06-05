using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FireScript : MonoBehaviour {

    [SerializeField]
    private Camera camFPS;

    [SerializeField]
    private LayerMask mask;

    [SerializeField] public Image hitMarker;

    public GunScript gun;

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
        if(Physics.Raycast(camFPS.transform.position, camFPS.transform.forward , out shot, gun.range, mask))
        {
            Debug.Log(" hit " + shot.collider.name);
            StopCoroutine("displayHitMarker");
            hitMarker.enabled=true;
            
            StartCoroutine("displayHitMarker");
        }
    }

    private IEnumerator displayHitMarker()
    {
        yield return new WaitForSeconds(0.15f);
        hitMarker.enabled=false;
    }
    
}
