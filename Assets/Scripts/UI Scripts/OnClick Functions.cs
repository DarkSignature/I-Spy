using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickFunctions : MonoBehaviourPunCallbacks
{
    public RoomListManager RoomManager;
    public TMP_Text RoomName;
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        if(RoomManager.active_rooms >= 3)
        {
            MainRoomUI.Instance.ShowNotification("There is already 3 active rooms. Please wait for them to finish before creating a new one.");
            return;
        }
        Debug.Log("Create Room clicked");
        MainNetwork.Instance.JoinOrCreateRoom("VR Room " + Random.Range(1000, 9999));

    }

    public void ToggleRoomPanel()
    {
         MainRoomUI.Instance.ToggleRoomList();
    }

    public void EnterGameScene()
    {
        Debug.Log("Entering Scene...");

        if (!MainNetwork.IsAdmin)
        {
             MainRoomUI.Instance.ShowNotification("Only admin can start the game room!");
            return;
        }

        photonView.RPC("RPC_MoveToGameScene", RpcTarget.AllBuffered);
    }

    public void StartGame()
    {
        Debug.Log("Start Game clicked");
        
        if (!MainNetwork.IsAdmin)
        {
            MainRoomUI.Instance.ShowNotification("Only admin can start the game!");
            return;
        }

        GameManager.Instance.StartGameForAll();
    }

    public void EndGame()
    {
        Debug.Log("End Game clicked");
        
        if (!MainNetwork.IsAdmin)
        {
            MainRoomUI.Instance.ShowNotification("Only admin can end the game!");
            return;
        }

        GameManager.Instance.AdminEndGame();
        PhotonNetwork.LeaveRoom();
    }

    public void CloseRoom()
    {
        Debug.Log("Close Room clicked");
        
        if (!MainNetwork.IsAdmin)
        {
            MainRoomUI.Instance.ShowNotification("Only admin can close the room!");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void CloseNotification()
    {
        MainRoomUI.Instance.CloseNotification();
        Debug.Log("Close Notif Clicked!");
    }

    public void JoinRoom()
    {
        MainNetwork.Instance.JoinOrCreateRoom(RoomName.text);
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
