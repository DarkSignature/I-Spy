using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();        
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinOrCreateRoom("VRRoom1", new RoomOptions{ MaxPlayers = 4}, null);
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined Room!");
        Debug.Log("Players in room: " + PhotonNetwork.PlayerList.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
