using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");

        PhotonNetwork.JoinOrCreateRoom(
            "VRRoom1",
            new RoomOptions { MaxPlayers = 4 },
            null
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        Debug.Log("Players in room: " + PhotonNetwork.PlayerList.Length);
    }

    public void MoveToGameScene()
    {
        photonView.RPC("RPC_MoveToGameScene", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_MoveToGameScene()
    {
        if (!MainNetwork.IsAdmin)
        {
            PhotonNetwork.LoadLevel("Game Room - Player");
        }
        else
        {
            PhotonNetwork.LoadLevel("Waiting Room - Admin");
        }
    }
}