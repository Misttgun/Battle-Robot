using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : Photon.PunBehaviour
{
	public static GameManagerScript Instance;

	public int alivePlayerNumber;
	
	private void Start ()
	{
		if(Instance == null)
			Instance = this;

		alivePlayerNumber = PhotonNetwork.room.PlayerCount;
		
		PhotonNetwork.Instantiate("RobotWheelNetwork", Vector3.zero, Quaternion.identity, 0);
	}
	
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}
	
	public void BackToLobby()
	{
		SceneManager.LoadScene(1);
	}
	
	public override void OnLeftRoom()
	{
		Invoke("BackToLobby", 3.5f);
	}
}
