using UnityEngine;
using UnityEngine.UI;
using Photon;

namespace BattleRobo
{
	/// <summary>
	/// Networked player class implementing movement control and shooting.
	/// Contains both server and client logic in an authoritative approach.
	/// </summary> 
	public class PlayerScript : PunBehaviour
	{
		[Header("Mouse Settings")]
		[SerializeField]
		/// <summary>
		/// Aim sensitivity.
		/// </summary>
		private float aimSensitivity = 5f;

		[Header("Movement Settings")]
		[SerializeField]
		/// <summary>
		/// Player walk speed.
		/// </summary>
		private float walkSpeed = 6.0f;

		[SerializeField]
		/// <summary>
		/// Player run speed.
		/// </summary>
		private float runSpeed = 11.0f;

		[SerializeField]
		/// <summary>
		/// Gravity applied to the player.
		/// </summary>
		private float gravity = 20.0f;

		[SerializeField]
		/// <summary>
		/// Speed at which the fuel decrease.
		/// </summary>
		private float fuelDecreaseSpeed = 0.1f;

		[SerializeField]
		/// <summary>
		/// Speed at which the fuel regenerate.
		/// </summary>
		private float fuelRegenSpeed = 0.05f;

		[SerializeField]
		/// <summary>
		/// Fly force for the jetpack.
		/// </summary>
		private float flyForce = 100f;

		[Header("Required Components")]
		[SerializeField]
		/// <summary>
		/// the charactercontroller for the player.
		/// </summary>
		private CharacterController controller;

		[SerializeField]
		/// <summary>
		/// The robo head transform for the camera rotation.
		/// </summary>
		private Transform roboHead;

		[SerializeField]
		/// <summary>
		/// A cached vrsion of the player photonview.
		/// </summary>
		private PhotonView myPhotonView;

		[SerializeField]
		/// <summary>
		/// The player animator.
		/// </summary>
		private Animator animator;

		[SerializeField]
		/// <summary>
		/// The thrusters gameobject for the flying effect.
		/// </summary>
		private GameObject thrusters;

		[SerializeField]
		/// <summary>
		/// The in game UI script.
		/// </summary>
		private PlayerUIScript uiScript;

		[SerializeField]
		/// <summary>
		/// The in game UI script.
		/// </summary>
		private GameObject playerUI;

		/// <summary>
		/// Last player gameobject that killed this one.
		/// </summary>
		[HideInInspector]
		public GameObject killedBy;

		private Vector3 moveDirection = Vector3.zero;
		private bool grounded;
		private float speed;
		public int maxHealth = 100;
		private float fuelAmount = 1f;
		private float maxFuelAmount = 1f;
		private Vector3 fly;
		private float currentRot;

		private Transform myTransform;

		public float FuelAmount
		{
			get { return fuelAmount; }
		}


		//Initialize server values for this player
		private void Awake()
		{
			//only let the master do initialization
			if (!PhotonNetwork.isMasterClient)
				return;

			//set players current health value after joining
			myPhotonView.SetHealth(maxHealth);
		}


		/// <summary>
		/// Initialize synced values on every client.
		/// Initialize camera and input for this local client.
		/// </summary>
		void Start()
		{
			//set name in label
			label.text = myPhotonView.GetName();
			//call hooks manually to update
			OnHealthChange(myPhotonView.GetHealth());
			OnShieldChange(myPhotonView.GetShield());

			//called only for this client 
			if (!photonView.isMine)
				return;

			//set a global reference to the local player
			GameManager.GetInstance().localPlayer = this;
		}


		/// <summary>
		/// This method gets called whenever player properties have been changed on the network.
		/// </summary>
		public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
		{
			//only react on property changes for this player
			PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
			if (player != photonView.owner)
				return;

			//update values that could change any time for visualization to stay up to date
			OnHealthChange(player.GetHealth());
			OnShieldChange(player.GetShield());
		}


		//this method gets called multiple times per second, at least 10 times or more
		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				//here we send the turret rotation angle to other clients
				stream.SendNext(turretRotation);
			}
			else
			{
				//here we receive the turret rotation angle from others and apply it
				this.turretRotation = (short)stream.ReceiveNext();
				OnTurretRotation();
			}
		}


		private void FixedUpdate()
		{
			//skip further calls for remote clients    
			if (!photonView.isMine)
			{
				//keep turret rotation updated for all clients
				OnTurretRotation();
				return;
			}

			//movement variables
			Vector2 moveDir;
			Vector2 turnDir;

			//reset moving input when no arrow keys are pressed down
			if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
			{
				moveDir.x = 0;
				moveDir.y = 0;
			}
			else
			{
				//read out moving directions and calculate force
				moveDir.x = Input.GetAxis("Horizontal");
				moveDir.y = Input.GetAxis("Vertical");
				Move(moveDir);
			}

			//cast a ray on a plane at the mouse position for detecting where to shoot 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(Vector3.up, Vector3.up);
			float distance = 0f;
			Vector3 hitPos = Vector3.zero;
			//the hit position determines the mouse position in the scene
			if (plane.Raycast(ray, out distance))
			{
				hitPos = ray.GetPoint(distance) - transform.position;
			}

			//we've converted the mouse position to a direction
			turnDir = new Vector2(hitPos.x, hitPos.z);

			//rotate turret to look at the mouse direction
			RotateTurret(new Vector2(hitPos.x, hitPos.z));

			//shoot bullet on left mouse click
			if (Input.GetButton("Fire1"))
				Shoot();
		}


		//moves rigidbody in the direction passed in
		void Move(Vector2 direction = default(Vector2))
		{
			//if direction is not zero, rotate player in the moving direction relative to camera
			if (direction != Vector2.zero)
				transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
									 * Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);

			//create movement vector based on current rotation and speed
			Vector3 movementDir = transform.forward * moveSpeed * Time.deltaTime;
			//apply vector to rigidbody position
			rb.MovePosition(rb.position + movementDir);
		}


		//on movement drag ended
		void MoveEnd()
		{
			//reset rigidbody physics values
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}


		//rotates turret to the direction passed in
		void RotateTurret(Vector2 direction = default(Vector2))
		{
			//don't rotate without values
			if (direction == Vector2.zero)
				return;

			//get rotation value as angle out of the direction we received
			turretRotation = (short)Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y;
			OnTurretRotation();
		}


		//on shot drag start set small delay for first shot
		void ShootBegin()
		{
			nextFire = Time.time + 0.25f;
		}


		//shoots a bullet in the direction passed in
		//we do not rely on the current turret rotation here, because we send the direction
		//along with the shot request to the server to absolutely ensure a synced shot position
		protected void Shoot(Vector2 direction = default(Vector2))
		{
			//if shot delay is over  
			if (Time.time > nextFire)
			{
				//set next shot timestamp
				nextFire = Time.time + fireRate;

				//send current client position and turret rotation along to sync the shot position
				//also we are sending it as a short array (only x,z - skip y) to save additional bandwidth
				short[] pos = new short[] { (short)(shotPos.position.x * 10), (short)(shotPos.position.z * 10) };
				//send shot request with origin to server
				this.photonView.RPC("CmdShoot", PhotonTargets.AllViaServer, pos, turretRotation);
			}
		}


		//called on the server first but forwarded to all clients
		[PunRPC]
		protected void CmdShoot(short[] position, short angle)
		{
			//get current bullet type
			int currentBullet = GetView().GetBullet();

			//calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
			//this is done to compensate network lag and smoothing it out between both client/server positions
			Vector3 shotCenter = Vector3.Lerp(shotPos.position, new Vector3(position[0] / 10f, shotPos.position.y, position[1] / 10f), 0.6f);
			Quaternion syncedRot = turret.rotation = Quaternion.Euler(0, angle, 0);

			//spawn bullet using pooling
			GameObject obj = PoolManager.Spawn(bullets[currentBullet], shotCenter, syncedRot);
			obj.GetComponent<Bullet>().owner = gameObject;

			//check for current ammunition
			//let the server decrease special ammunition, if present
			if (PhotonNetwork.isMasterClient && currentBullet != 0)
			{
				//if ran out of ammo: reset bullet automatically
				GetView().DecreaseAmmo(1);
			}

			//send event to all clients for spawning effects
			if (shotFX || shotClip)
				RpcOnShot();
		}


		//called on all clients after bullet spawn
		//spawn effects or sounds locally, if set
		protected void RpcOnShot()
		{
			if (shotFX) PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
			if (shotClip) AudioManager.Play3D(shotClip, shotPos.position, 0.1f);
		}


		//hook for updating turret rotation locally
		void OnTurretRotation()
		{
			//we don't need to check for local ownership when setting the turretRotation,
			//because OnPhotonSerializeView PhotonStream.isWriting == true only applies to the owner
			turret.rotation = Quaternion.Euler(0, turretRotation, 0);
		}


		//hook for updating health locally
		//(the actual value updates via player properties)
		protected void OnHealthChange(int value)
		{
			healthSlider.value = (float)value / maxHealth;
		}


		//hook for updating shield locally
		//(the actual value updates via player properties)
		protected void OnShieldChange(int value)
		{
			shieldSlider.value = value;
		}


		/// <summary>
		/// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
		/// </summary>
		public void TakeDamage(Bullet bullet)
		{
			//store network variables temporary
			int health = GetView().GetHealth();
			int shield = GetView().GetShield();

			//reduce shield on hit
			if (shield > 0)
			{
				GetView().DecreaseShield(1);
				return;
			}

			//substract health by damage
			//locally for now, to only have one update later on
			health -= bullet.damage;

			//bullet killed the player
			if (health <= 0)
			{
				//the game is already over so don't do anything
				if (GameManager.GetInstance().IsGameOver()) return;

				//get killer and increase score for that team
				Player other = bullet.owner.GetComponent<Player>();
				int otherTeam = other.GetView().GetTeam();
				PhotonNetwork.room.AddScore(otherTeam);

				//the maximum score has been reached now
				if (GameManager.GetInstance().IsGameOver())
				{
					//close room for joining players
					PhotonNetwork.room.IsOpen = false;
					//tell all clients the winning team
					this.photonView.RPC("RpcGameOver", PhotonTargets.All, (byte)otherTeam);
					return;
				}

				//the game is not over yet, reset runtime values
				//also tell all clients to despawn this player
				GetView().SetHealth(maxHealth);
				GetView().SetBullet(0);
				this.photonView.RPC("RpcRespawn", PhotonTargets.All);
			}
			else
			{
				//we didn't die, set health to new value
				GetView().SetHealth(health);
			}
		}


		//called on all clients on both player death and respawn
		//only difference is that on respawn, the client sends the request
		[PunRPC]
		protected virtual void RpcRespawn()
		{
			//toggle visibility for player gameobject (on/off)
			gameObject.SetActive(!gameObject.activeInHierarchy);
			bool isActive = gameObject.activeInHierarchy;

			//the player has been killed
			if (!isActive)
			{
				//detect whether the current user was responsible for the kill
				//yes, that's my kill: increase local kill counter & take thumbnail via EveryPlay
				if (killedBy == GameManager.GetInstance().localPlayer.gameObject)
				{
					GameManager.GetInstance().ui.killCounter[0].text = (int.Parse(GameManager.GetInstance().ui.killCounter[0].text) + 1).ToString();
					GameManager.GetInstance().ui.killCounter[0].GetComponent<Animator>().Play("Animation");
					UnityEveryplayManager.TakeThumbnail();
				}

				if (explosionFX)
				{
					//spawn death particles locally using pooling and colorize them in the player's team color
					GameObject particle = PoolManager.Spawn(explosionFX, transform.position, transform.rotation);
					ParticleColor pColor = particle.GetComponent<ParticleColor>();
					if (pColor) pColor.SetColor(GameManager.GetInstance().teams[GetView().GetTeam()].material.color);
				}

				//play sound clip on player death
				if (explosionClip) AudioManager.Play3D(explosionClip, transform.position);
			}

			//further changes only affect the local client
			if (!photonView.isMine)
				return;

			//local player got respawned so reset states
			if (isActive == true)
				ResetPosition();
			else
			{
				//local player was killed, set camera to follow the killer
				camFollow.target = killedBy.transform;
				//hide input controls and other HUD elements
				camFollow.HideMask(true);
				//display respawn window (only for local player)
				GameManager.GetInstance().DisplayDeath();
			}
		}


		/// <summary>
		/// Command telling the server and all others that this client is ready for respawn.
		/// This is when the respawn delay is over or a video ad has been watched.
		/// </summary>
		public void CmdRespawn()
		{
			this.photonView.RPC("RpcRespawn", PhotonTargets.AllViaServer);
		}


		/// <summary>
		/// Repositions in team area and resets camera & input variables.
		/// This should only be called for the local player.
		/// </summary>
		public void ResetPosition()
		{
			//start following the local player again
			camFollow.target = turret;
			camFollow.HideMask(false);

			//get team area and reposition it there
			transform.position = GameManager.GetInstance().GetSpawnPosition(GetView().GetTeam());

			//reset forces modified by input
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			transform.rotation = Quaternion.identity;
			//reset input left over
			GameManager.GetInstance().ui.controls[0].OnEndDrag(null);
			GameManager.GetInstance().ui.controls[1].OnEndDrag(null);
		}


		//called on all clients on game end providing the winning team
		[PunRPC]
		protected void RpcGameOver(byte teamIndex)
		{
			//display game over window
			GameManager.GetInstance().DisplayGameOver(teamIndex);
		}
	}
}