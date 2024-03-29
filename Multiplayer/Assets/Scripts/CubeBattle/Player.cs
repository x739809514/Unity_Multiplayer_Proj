using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class Player : NetworkBehaviour
{
    public float moevSpeed;
    public float turnSpeed;
    public Text textId;
    private float horizontal;
    private float vertical;
    private Color[] colors = { Color.black, Color.white, Color.red };
    [SerializeField]
    private ulong[] touchIds;

    // NetworkVariable is used to synchornize variable between client and server
    private NetworkVariable<Vector3> vPos = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> vRot = new NetworkVariable<Quaternion>();
    private NetworkVariable<int> clientId = new NetworkVariable<int>();
    private Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(-5, 5), 3, Random.Range(-5, 5));
        }
        textId.text = "id: " + clientId.Value.ToString();
        Renderer material = this.GetComponent<MeshRenderer>();
        material.material.color = colors[(int)clientId.Value % colors.Length];
    }

    // this method will be called when an object is spawned
    // it will be called before Start()
    public override void OnNetworkSpawn()
    {
        if (this.IsServer)
        {
            clientId.Value = (int)this.OwnerClientId;
            Debug.Log("Server--The new object's id is: " + this.OwnerClientId);
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Move(out var targetPos);
            Rotate(out var target);

            SubmitPositionRequestServerRpc(targetPos, target);
        }
        else
        {
            transform.position = vPos.Value;
            transform.rotation = vRot.Value;
        }

    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestServerRpc(Vector3 pos, Quaternion rot)
    {
        vPos.Value = pos;
        vRot.Value = rot;
    }

    private void Move(out Vector3 targetPos)
    {
        Vector3 delta = this.transform.forward * vertical * moevSpeed * Time.deltaTime;
        targetPos = rb.position + delta;
        rb.MovePosition(targetPos);
    }

    private void Rotate(out Quaternion target)
    {
        Quaternion delta = Quaternion.Euler(0, horizontal * turnSpeed * Time.deltaTime, 0);
        target = rb.rotation * delta;
        rb.MoveRotation(target);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            var coin = other.gameObject.GetComponent<Coin>();
            coin.SetCoinActive(false);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (this.IsClient && this.IsOwner)
            {
                var targetClientId = other.gameObject.GetComponent<NetworkObject>().OwnerClientId;
                if (touchIds == null) touchIds = new ulong[] { targetClientId };
                OnCollisionWithPlayerRpc(this.OwnerClientId, touchIds);
            }
        }
    }

    // in Rpc system, the method will be called on server version and local version
    // in  this case, "OnCollisionWithPlayerRpc" is also excuted in server version
    [Rpc(SendTo.Server)]
    private void OnCollisionWithPlayerRpc(ulong fromId, ulong[] toId)
    {
        ClientRpcParams rpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = toId
            }
        };
        NoticeCollisionToClientRpc(fromId);
    }

    // Server send notify to a specific client, not every clients
    [Rpc(SendTo.ClientsAndHost)]
    private void NoticeCollisionToClientRpc(ulong id)
    {
        if (!this.IsOwner && this.OwnerClientId == id)
        {
            Debug.Log("the target player's id is: " + id);
        }
    }
}
