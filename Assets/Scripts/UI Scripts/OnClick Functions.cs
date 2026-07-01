using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickFunctions : MonoBehaviourPunCallbacks
{
    public MainRoomUI UIManager;
    public MainNetwork NetworkManager;
    public RoomListManager RoomManager;
    public TMP_Text RoomName;
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        if(RoomManager.active_rooms >= 3)
        {
            UIManager.ShowNotification("There is already 3 active rooms. Please wait for them to finish before creating a new one.");
            return;
        }
        Debug.Log("Create Room clicked");
        MainNetwork.Instance.JoinOrCreateRoom("VR Room " + Random.Range(1000, 9999));

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

    public void JoinRoom()
    {
        
        MainNetwork.Instance.JoinOrCreateRoom(RoomName.text);
    }
}
