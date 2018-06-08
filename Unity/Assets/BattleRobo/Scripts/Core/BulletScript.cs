using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	[SerializeField]
	private int speed;

	public Vector3 target;
	public Vector3 spawnPoint;

	private Vector3 direction;

	// Update is called once per frame
	private void Update ()
	{
		direction = target - spawnPoint;
		transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
	}
}
