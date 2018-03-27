using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{

	/// <summary>
	/// The target tranform the camera is going to follow.
	/// </summary>
	public Transform target;

	/// <summary>
	/// The camera transform.
	/// </summary>
	private Transform camTransform;

	/// <summary>
	/// The distance between the camera and the target.
	/// </summary>
	public float distance = 5f;

	private void Start()
	{
		camTransform = transform;
	}

	private void LateUpdate()
	{
		
	}

	private void FixedUpdate()
	{
		var camPosition = target.position + distance * -camTransform.forward;

		RaycastHit hit;
		if (Physics.Linecast(target.position, camPosition, out hit))
		{
			var hitPoint = new Vector3(hit.point.x + hit.normal.x * 0.2f, hit.point.y, hit.point.z + hit.normal.z * 0.2f);
			camPosition = new Vector3(hitPoint.x, camPosition.y, hit.point.z);
		}

		camTransform.position = camPosition;
		camTransform.rotation = target.rotation;
	}

	public void SetTarget(Transform camTarget)
	{
		target = camTarget;
	}
}
