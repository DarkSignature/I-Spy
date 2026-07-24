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

    public static bool IsAdmin { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        
        // First player in the room becomes admin
        IsAdmin = PhotonNetwork.PlayerList.Length == 1;
        
        Debug.Log("IsAdmin: " + IsAdmin);
    }

    public void JoinOrCreateRoom(String roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions{ MaxPlayers = 5}, null);
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

    [PunRPC]
    public void RPC_SetQuestion(int ID)
    {
        GameManager.Instance.currentQuestion =
        QuestionManager.Instance.GetQuestionByID(ID);

        if (GameManager.Instance.CurrentState == GameState.Animation)
        {
            GameManager.Instance.EnterAnimation(GameManager.Instance.currentQuestion);
        }
    }

    [PunRPC]
    public void RPC_NextQuestion()
    {
        GameManager.Instance.EndRound();
    }
}
