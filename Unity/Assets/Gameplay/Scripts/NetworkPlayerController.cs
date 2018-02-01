using BattleRobo.Networking;
using Photon;
using UnityEngine;

public class NetworkPlayerController : PunBehaviour
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
    
    private const ushort MaxHealth = 100;

    [Header("Required Components")] [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject cam;

    [Header("Interpolation Settings")]
    // How far back to rewind interpolation
    [SerializeField] private float interpolationBackTime = 0.1f;

    // We'll keep a buffer of 20 network states
    private NetworkState[] stateBuffer = new NetworkState[20];

    // The number of states that have been recorded
    private int stateCount;

    public bool IsDead { get; private set; }
    public ushort CurrentHealth { get; private set; }
    
    

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
                
            }
        }

    }

    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 position = Vector3.zero;
        if (stream.isWriting)
        {
            position = transform.position;
            stream.Serialize(ref position);
        }
        else
        {
            stream.Serialize(ref position);
            BufferState(new NetworkState(position, info.timestamp));
        }
    }

    // save new state to buffer
    /// <summary>
    /// Save a new sate to the buffer.
    /// </summary>
    /// <param name="state"></param>
    private void BufferState(NetworkState state)
    {
        // Shift buffer contents to accommodate new state
        for (int i = stateBuffer.Length - 1; i > 0; i--)
        {
            stateBuffer[i] = stateBuffer[i - 1];
        }

        // Save state to slot 0
        stateBuffer[0] = state;

        // Increment state count
        stateCount = Mathf.Min(stateCount + 1, stateBuffer.Length);
    }
}