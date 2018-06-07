
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorScript : MonoBehaviour
{
    [SerializeField] private Camera cam;
    
    [SerializeField] private Transform whereShotComeFrom;

    [SerializeField] private Image damageIndicator;

    //[SerializeField] private Image arrowIndicator;

    

    private Vector3 pointing;

    private void Update()
    {
        indicator();
    }

    private void indicator()
    {
        
        Vector3 direction = cam.WorldToScreenPoint(whereShotComeFrom.transform.position);   // donne la position de la personne qui vous a tiré en fonction du champ du vision de la caméra
       
        pointing.z = Mathf.Atan2((-damageIndicator.transform.position.y), (damageIndicator.transform.position.x-direction.x)) * Mathf.Rad2Deg+180 ;//  damage indicator indique la position de l'ennemie devant nous
        
        // si la valeur de z est négatif c'est que l'ennemie n'est pas dans le champ de vision 
        if (direction.z < 0) 
        {
            pointing.z = pointing.z + 180; // faire en sorte que le damage indicator indique la position de l'ennemie derrière nous 
        }
        damageIndicator.transform.rotation = Quaternion.Euler(pointing);
    }
    
    
    private IEnumerator displayDamageIndicator()
    {
        yield return new WaitForSeconds(0.15f);
        damageIndicator.enabled = false;
    }

    
}
