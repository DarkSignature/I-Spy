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

    private bool refreshingLobby = false;

    /// <summary>
    /// Forces a full room list resend by leaving and rejoining the lobby.
    /// Photon only sends deltas while in a lobby, so this is the reliable way
    /// to drop stale rooms from previous sessions and pick up the current ones.
    /// </summary>
    public void RefreshRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            refreshingLobby = true;
            PhotonNetwork.LeaveLobby();
        }
        else if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnLeftLobby()
    {
        if (refreshingLobby)
        {
            refreshingLobby = false;
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby - waiting for full room list");
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined Room!");
        Debug.Log("Players in room: " + PhotonNetwork.PlayerList.Length);

        // First player in the room becomes admin
        IsAdmin = PhotonNetwork.PlayerList.Length == 1;

        // Publish admin flag so the leaderboard can filter the admin out
        var props = new ExitGames.Client.Photon.Hashtable { { "isAdmin", IsAdmin } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

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

    /// <summary>
    /// Admin broadcasts the chosen question + shuffle seed so every client
    /// shows the same question and identical animal placement.
    /// </summary>
    [PunRPC]
    public void RPC_SetQuestion(int questionID, int shuffleSeed)
    {
        Question question = QuestionManager.Instance.GetQuestionByID(questionID);

        if (question == null)
        {
            Debug.LogError($"RPC_SetQuestion: no question with questionID {questionID}!");
            return;
        }

        GameManager.Instance.currentQuestion = question;
        GameManager.Instance.SetShuffleSeed(shuffleSeed);

        if (GameManager.Instance.CurrentState == GameState.Animation)
        {
            GameManager.Instance.EnterAnimation(question);
        }
    }

    /// <summary>
    /// Joins an existing room from the lobby list. Unlike JoinOrCreateRoom,
    /// this fails loudly (OnJoinRoomFailed) instead of silently creating
    /// a new empty room when the name doesn't match.
    /// </summary>
    public void JoinRoom(String roomName)
    {
        Debug.Log("Requesting join: " + roomName);

        if (!PhotonNetwork.JoinRoom(roomName))
        {
            Debug.LogError("Join request could not be sent - client is not ready (still connecting?)");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed ({returnCode}): {message}");

        if (MainRoomUI.Instance != null)
            MainRoomUI.Instance.ShowNotification("Failed to join room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Create room failed ({returnCode}): {message}");

        if (MainRoomUI.Instance != null)
            MainRoomUI.Instance.ShowNotification("Failed to create room: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from Photon: " + cause);
    }

    [PunRPC]
    public void RPC_NextQuestion()
    {
        GameManager.Instance.EndRound();
    }
}
