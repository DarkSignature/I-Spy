using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickFunctions : MonoBehaviourPunCallbacks
{
    public MainRoomUI UIManager;
    public MainNetwork NetworkManager;
    public RoomListManager RoomManager;
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        if(RoomManager.active_rooms >= 3)
        {
            UIManager.ShowNotification("There is already 3 active rooms. Please wait for them to finish before creating a new one.");
        }
        Debug.Log("Create Room clicked");
        NetworkManager.CreateRoom();
    }

    public void ToggleRoomPanel()
    {
        UIManager.ToggleRoomList();
    }

    public void StartGame()
    {
        Debug.Log("Start Game clicked");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CloseNotification()
    {
        UIManager.CloseNotification();
        Debug.Log("Close Notif Clicked!");
    }
}
