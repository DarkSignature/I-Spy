using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomCard : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    public TMP_Text roomStatus;

    private string roomName;

    /// <summary>True while this room still has a free player slot.</summary>
    public bool HasSpace { get; private set; }

    public void Setup(RoomInfo room)
    {
        roomName = room.Name;

        roomNameText.text = room.Name;

        HasSpace = room.IsOpen && room.PlayerCount < room.MaxPlayers;

        roomStatus.text = HasSpace ? "Waiting for Players" : "Full";

        playerCountText.text =
            room.PlayerCount - 1 +
            "/" +
            room.MaxPlayers;

        // Full rooms can't be clicked
        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = HasSpace;
    }

    /// <summary>
    /// Joins the room this card represents, using the name straight from
    /// RoomInfo (never the displayed UI text). Wired to the card's button
    /// via OnClickFunctions.JoinRoom.
    /// </summary>
    public void JoinThisRoom()
    {
        Debug.Log("Room card clicked, joining: " + roomName);
        MainNetwork.Instance.JoinRoom(roomName);
    }
}
