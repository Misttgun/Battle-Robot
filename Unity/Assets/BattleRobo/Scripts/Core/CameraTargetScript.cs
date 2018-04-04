using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
	public class CameraTargetScript : MonoBehaviour
	{

		/*[SerializeField]
		private PlayerScript player;*/
		
		[SerializeField]
		private PlayerControllerScript player;

		private void Update()
		{
			transform.localEulerAngles = new Vector3(player.currentRot, 0f, 0f);
		}
	}
}


