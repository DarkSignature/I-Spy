using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainRoomUI : MonoBehaviourPunCallbacks
{
    public GameObject mainMenuPanel;
    public GameObject waitingRoomPanel;
    public GameObject roomListPanel;
    public GameObject notifPanel;
    public TMP_Text roomNameText;
    public TMP_Text notifMsg;
    public TMP_Text playerCountText;
    public static MainRoomUI Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        mainMenuPanel.SetActive(true);
        waitingRoomPanel.SetActive(false);
        roomListPanel.SetActive(false);
        notifPanel.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        mainMenuPanel.SetActive(false);
        roomListPanel.SetActive(false);
        notifPanel.SetActive(false);
        waitingRoomPanel.SetActive(true);
        if (roomNameText != null)
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerCount();
        
        // Update admin UI for waiting room
        if (WaitingRoomUI.Instance != null)
            WaitingRoomUI.Instance.UpdateAdminUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
    }

    void UpdatePlayerCount()
    {
            if (playerCountText == null)
                return;
            
        playerCountText.text =
            "Player(s) - "
            +
            (PhotonNetwork.CurrentRoom.PlayerCount - 1)
            + " / " +
            "4";
    }
    public void ShowNotification(string message)
    {
        notifMsg.text = message;
        notifPanel.SetActive(true);
    }

    public void CloseNotification()
    {
        notifPanel.SetActive(false);
    }
    public void ToggleRoomList()
    {
        if(roomListPanel.activeSelf == false)
        {
            mainMenuPanel.SetActive(false);
            roomListPanel.SetActive(true);
        }
        else
        {
            mainMenuPanel.SetActive(true);
            roomListPanel.SetActive(false);
        }
    }
}