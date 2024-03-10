using Unity.Netcode;
using UnityEngine;

public class RPCTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner)
        {
            TestServerRpc(0,NetworkObjectId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Receive the RPC {value} on NetworkObject {sourceNetworkObjectId}");
        if (IsOwner)
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server REceive the RPC {value} on NetworkObject {sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
}