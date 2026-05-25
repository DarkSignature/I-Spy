using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MainNetwork : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static MainNetwork Instance;

    void Awake()
    {
        Instance = this;
    }
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

    public void JoinOrCreateRoom(String roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions{ MaxPlayers = 5}, null);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
