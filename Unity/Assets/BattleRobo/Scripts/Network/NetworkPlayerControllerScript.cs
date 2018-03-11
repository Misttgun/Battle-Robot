using System.Collections.Generic;
using BattleRobo.Core;
using BattleRobo.Networking;
using Photon;
using UnityEngine;

public class NetworkPlayerControllerScript : PunBehaviour
{
    // A struct of values received over the network
    private struct NetworkState
    {
        public NetworkState(Vector3 pos, double time) : this()
        {
            Position = pos;
            Timestamp = time;
        }

        public Vector3 Position { get; private set; }
        public double Timestamp { get; private set; }
    }

    // A movement command sent to the server
    private struct MoveCommand
    {
        public MoveCommand(float horiz, float vert, double timestamp) : this()
        {
            HorizontalAxis = horiz;
            VerticalAxis = vert;
            Timestamp = timestamp;
        }

        public float HorizontalAxis { get; private set; }
        public float VerticalAxis { get; private set; }
        public double Timestamp { get; private set; }
    }

    [Header("Required Components")]
    [SerializeField]
    private PlayerControllerScript playerController;

    [SerializeField]
    private GameObject playerUI;

    [SerializeField]
    private GameObject cam;

    [Header("Interpolation Settings")]
    // How far back to rewind interpolation
    [SerializeField]
    private float interpolationBackTime = 0.1f;

    // We'll keep a buffer of 20 network states
    private readonly NetworkState[] stateBuffer = new NetworkState[20];

    // The number of states that have been recorded
    private int stateCount;

    // A list of move states sent from client to server. We reserve 201 because 200 is our cap.
    private readonly List<MoveCommand> movementHistory = new List<MoveCommand>(201);

    private float updateTimer;
    private ushort health = 100;

    public bool IsDead { get; private set; }
    
    public ushort Health
    {
        get { return health; }
    }


    private void Start()
    {
        if (cam != null)
        {
            if (photonView.isMine)
            {
                var spawns = GameManagerScript.Instance.spawnPoints;

                cam.SetActive(true);
                IsDead = false;
                playerUI.SetActive(true);

                transform.position = spawns[Random.Range(0, spawns.Length)].position;
            }
        }
    }

    private void Update()
    {
        // Are we the master client
        if (PhotonNetwork.isMasterClient)
        {
            // We send out position update every 1/10th of a second
            updateTimer += Time.deltaTime;
            if (updateTimer >= 0.1f)
            {
                updateTimer = 0f;
                photonView.RPC("NetworkUpdate", PhotonTargets.Others, transform.position);
            }
        }
        
        // No interpolation if we are the local player
        if (photonView.isMine)
        {
            return;
        }

        // No interpolation if there is nothing to interpolate
        if (stateCount == 0)
        {
            return;
        }

        double currentTime = Network.time;
        double interpolationTime = currentTime - interpolationBackTime;

        // If the latest packet is newer than interpolationTime, we can interpolate
        if (stateBuffer[0].Timestamp > interpolationTime)
        {
            for (int i = 0; i < stateCount; i++)
            {
                // Find the closest state that matches network time or use oldest state
                if (stateBuffer[i].Timestamp <= interpolationTime || i == stateCount - 1)
                {
                    // The state closest to network time
                    NetworkState lhs = stateBuffer[i];

                    // The state one slot newer
                    NetworkState rhs = stateBuffer[Mathf.Max(i - 1, 0)];

                    // We use the time between lhs and rhs to interpolate
                    double length = rhs.Timestamp - lhs.Timestamp;

                    float time = 0f;
                    if (length > 0.0001)
                    {
                        time = (float) ((interpolationTime - lhs.Timestamp) / length);
                    }

                    transform.position = Vector3.Lerp(lhs.Position, rhs.Position, time);
                    break;
                }
            }
        }
    }

    
    [PunRPC]
    public void TakeDamage(ushort amount)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            // player is dead
            Die();
        }
    }


    private void Die()
    {
        var photonId = photonView.instantiationId;

        if (photonId == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManagerScript.Instance.alivePlayerNumber--;
            if (!photonView.isMine)
            {
                return;
            }
            IsDead = true;
            GameManagerScript.Instance.ShowGameOverScreen("You died... Feels bad man !!");
        }
    }
}