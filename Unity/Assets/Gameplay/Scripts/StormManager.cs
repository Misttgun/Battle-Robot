using System.Collections;
using System.Collections.Generic;
using BattleRobo.Networking;
using UnityEngine;

public class StormManager : MonoBehaviour
{
	[SerializeField] private GameObject storm; 
	[SerializeField] private float sizing;
	[SerializeField] private int stormDmg;
	[SerializeField] private float waitTime;
	[SerializeField] LevelGeneratorScript mapGenerator;
	
	private float stormSize; // à calculer avec la taille de la map generator
	private Vector3 size;
	private float lerpTime=1f;
	private float currentLerpTime;

	private bool lerping;

	private float startSize;
	private float endSize;

	private bool stormActive;
	
	void Start ()
	{
		stormTransform();  // donner la taille de départ de la zone
		StartCoroutine(stormManageScale());
	}
	
	private void Update()
	{
		if (lerping)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime>lerpTime)
			{
				currentLerpTime = lerpTime;
				lerping = false;
			}

			float ratio = currentLerpTime / lerpTime;
			stormSize = Mathf.Clamp(Mathf.Lerp(startSize, endSize, ratio), 10f, 1000f);//1000f peut etre changer
			transform.localScale =new Vector3(stormSize,transform.localScale.y,stormSize);
		}
	}

	IEnumerator stormManageScale()
	{
		stormActive = true;
		while(stormActive)
		{
			startSize = stormSize;
			endSize = stormSize - sizing;
			lerping = true;
			currentLerpTime = 0;
			
			yield return new WaitForSeconds(waitTime);
		}
	}

	private void stormTransform()				//taille de la storm
	{
		int m = mapGenerator.getMapMainSize();
		float h = mapGenerator.getHeight();
		stormSize = m*3/2;
		float stormCenterPos = (m)/ 2;
		transform.localScale =new Vector3(stormSize,h*2,stormSize);
		transform.position =new Vector3(stormCenterPos,h/2,stormCenterPos);
	}
	// If Robo is outside the Safe-Zone
	//apply storm damage
	private void OnTriggerExit(Collider other)
	{
		
		if (other.CompareTag("Player"))
		{
			Debug.LogWarning("dans la zone ");
			other.GetComponent<StormPlayerController>().StormApplyDmg(stormDmg);
		}
	}
	
	// If Robo is inside the Safe-Zone
	//stop storm damage
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.LogWarning("dans la safe-zone ");
			other.GetComponent<StormPlayerController>().StormStopDmg();
		}
	}
}
