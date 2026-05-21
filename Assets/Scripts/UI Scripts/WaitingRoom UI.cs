using Photon.Pun;
using TMPro;
using UnityEngine;

public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }
}