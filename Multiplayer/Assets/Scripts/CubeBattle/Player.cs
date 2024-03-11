using UnityEngine;
using Unity.Netcode;


public class Player : NetworkBehaviour
{
    public float moevSpeed;
    public float turnSpeed;
    private float horizontal;
    private float vertical;
    private NetworkVariable<Vector3> vPos = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> vRot = new NetworkVariable<Quaternion>();
    private Rigidbody rb;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(-5, 5), 3, Random.Range(-5, 5));
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
}
