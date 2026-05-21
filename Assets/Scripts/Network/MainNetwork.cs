using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainNetwork : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting");
        PhotonNetwork.ConnectUsingSettings();        
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined Room!");
        Debug.Log("Players in room: " + PhotonNetwork.PlayerList.Length);
    }

    public void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("VR Room " + Random.Range(1000, 9999), new RoomOptions{ MaxPlayers = 4}, null);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
