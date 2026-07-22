using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainRoomUI : MonoBehaviourPunCallbacks
{
    public GameObject mainMenuPanel;
    public GameObject waitingRoomPanel;
    public GameObject roomListPanel;
    public GameObject notifPanel;
    public TMP_Text roomNameText;
    public TMP_Text notifMsg;
    public TMP_Text playerCountText;

    [Tooltip("Player-only: button that opens the room list. Disabled until a joinable room exists. Leave empty on the admin scene.")]
    public Button startPlayingButton;

    [Tooltip("Optional: text on the waiting room panel, e.g. 'Waiting for admin to start the game...'")]
    public TMP_Text waitingStatusText;

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

        // Bright white when a room is available (same as the other buttons),
        // clearly dimmed while there is nothing to join.
        if (startPlayingButton != null)
        {
            ColorBlock colors = startPlayingButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 1f);
            startPlayingButton.colors = colors;
        }

        // Locked until the lobby reports a room with a free slot
        SetRoomAvailability(false);
    }

    /// <summary>
    /// Called by RoomListManager whenever the lobby's room list changes.
    /// </summary>
    public void SetRoomAvailability(bool hasAvailableRoom)
    {
        if (startPlayingButton != null)
            startPlayingButton.interactable = hasAvailableRoom;
    }

    public override void OnJoinedRoom()
    {
        mainMenuPanel.SetActive(false);
        roomListPanel.SetActive(false);
        notifPanel.SetActive(false);
        waitingRoomPanel.SetActive(true);
        if (roomNameText != null)
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        if (waitingStatusText != null)
        {
            waitingStatusText.text = MainNetwork.IsAdmin
                ? "You are the admin - start the game when everyone is ready!"
                : "Waiting for admin to start the game...";
        }

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