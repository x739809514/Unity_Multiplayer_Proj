using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public GameObject coinPrefab;
    public int coinsNumber;

    public Button btnServer;
    public Button btnClient;
    public Button btnHost;
    public Button btnShutdown;


    private void Start()
    {
        btnServer.onClick.AddListener(ServerOnClick);
        btnClient.onClick.AddListener(ClientOnClick);
        btnHost.onClick.AddListener(HostOnClick);
        btnShutdown.onClick.AddListener(ShutDownOnClick);

        NetworkManager.Singleton.OnClientConnectedCallback += onClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnect;
        NetworkManager.Singleton.OnServerStarted+=()=>{SpawnCoins();};
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < coinsNumber; i++)
        {
            var coin = Instantiate<GameObject>(coinPrefab);
            coin.gameObject.GetComponent<NetworkObject>().Spawn();
            coin.transform.position = new Vector3(i + 2, 3, i - 2);
            coin.SetActive(true);
        }
    }

    #region Callback
    private void ServerOnClick()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server start success");
        }
        else
        {
            Debug.Log("Server start failed");
        }
    }

    private void ClientOnClick()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client start success");
        }
        else
        {
            Debug.Log("Client start failed");
        }
    }

    private void HostOnClick()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host start success");
        }
        else
        {
            Debug.Log("Host start failed");
        }
    }

    private void ShutDownOnClick()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void onClientConnected(ulong id)
    {
        Debug.Log("Client Connect!_" + id);
    }

    private void onClientDisconnect(ulong id)
    {
        Debug.Log("Client Disconnect!_" + id);
    }
    #endregion


}