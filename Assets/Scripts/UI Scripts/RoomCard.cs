using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomCard : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    public TMP_Text roomStatus;

    private string roomName;

    public void Setup(RoomInfo room)
    {
        roomName = room.Name;

        roomNameText.text = room.Name;
        
        roomStatus.text = "Waiting for Players";

        playerCountText.text =
            room.PlayerCount - 1 +
            "/" +
            room.MaxPlayers;
    }
}
