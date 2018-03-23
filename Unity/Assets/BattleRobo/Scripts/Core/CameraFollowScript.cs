using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
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
	/// The offset between the player and the camera;
	/// </summary>
	private Vector3 offset = new Vector3(0f, 0f, -0.05f);

	/// <summary>
	/// The mouse sensitivity.
	/// </summary>
	private float sentivity = 2f;

	// The amount of vertical rotation to apply to the player and camera
	public float verticalRot;

	private void Start()
	{
		camTransform = transform;

		// We place the holder of this script at the same position as the target.
		camTransform.position = target.position;
		camTransform.rotation = target.rotation;
	}

	private void LateUpdate()
	{
		verticalRot -= Input.GetAxisRaw("Mouse Y") * sentivity;
		verticalRot = Mathf.Clamp(verticalRot, -60f, 60f);

		float desiredAngle = target.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(verticalRot, desiredAngle, 0f);
		camTransform.position = target.position + rotation * offset;

		camTransform.LookAt(target);
	}
}
