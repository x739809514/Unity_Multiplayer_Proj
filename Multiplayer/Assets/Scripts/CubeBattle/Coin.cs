
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    private NetworkVariable<bool> coinIsActive = new NetworkVariable<bool>(true);

    public override void OnNetworkSpawn()
    {
        coinIsActive.OnValueChanged += (preValue, newValue) =>
        {
            this.gameObject.SetActive(newValue);
        };
        this.gameObject.SetActive(coinIsActive.Value);
    }

    public void SetCoinActive(bool isActive)
    {
        if (IsServer)
        {
            coinIsActive.Value = isActive; 
        }
        else if (IsClient)
        {
            SubmitActiveRequestServerRpc(isActive);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SubmitActiveRequestServerRpc(bool isActive)
    {
        coinIsActive.Value = isActive;
    }
}