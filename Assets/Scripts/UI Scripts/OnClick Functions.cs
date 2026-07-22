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

        // Opening the list? Force a fresh full room list from Photon so
        // stale rooms from earlier sessions never linger on screen.
        if (MainRoomUI.Instance.roomListPanel.activeSelf)
        {
            MainNetwork.Instance.RefreshRoomList();
        }
    }

    public void EnterGameScene()
    {
        Debug.Log("Entering Scene...");

        if (!MainNetwork.IsAdmin)
        {
             MainRoomUI.Instance.ShowNotification("Only admin can start the game room!");
            return;
        }

        MainNetwork.Instance.photonView.RPC("RPC_MoveToGameScene", RpcTarget.All);
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
        // On a room card this component sits next to RoomCard - join using
        // its stored room name instead of the displayed UI text.
        RoomCard card = GetComponent<RoomCard>();

        if (card != null)
        {
            card.JoinThisRoom();
            return;
        }

        MainNetwork.Instance.JoinRoom(RoomName.text.Trim());
    }

}
