
using UnityEngine;

public class DamageIndicatorScript : MonoBehaviour
{
    [SerializeField] private Camera cam;
    
    [SerializeField] private Transform whereShotComeFrom;

    [SerializeField] private Transform indicatorArrow;

    private Vector3 pointing;

    private void update()
    {
        indicator();
    }

    private void indicator()
    {
        Vector3 direction = cam.WorldToScreenPoint(whereShotComeFrom.transform.position);
        pointing.z =
            Mathf.Atan2((indicatorArrow.transform.position.y - direction.y), (indicatorArrow.transform.position.x)) *
            Mathf.Rad2Deg - 90;
        indicatorArrow.transform.rotation = Quaternion.Euler(pointing);
    }

}
