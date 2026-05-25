using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public int active_rooms = 0;
    public GameObject roomCardPrefab;
    public Transform contentParent;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach(RoomInfo room in roomList)
        {
            if(room.RemovedFromList)
                continue;

            GameObject card =
                Instantiate(roomCardPrefab, contentParent);

            card.transform.localScale = Vector3.one;
            
            card.GetComponent<RoomCard>()
                .Setup(room);

            Debug.Log("Created Room Card!");
        }

        active_rooms = 0;
        foreach(RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)
            {
                active_rooms++;
            }
        }
        Debug.Log("Number of active_rooms: " + active_rooms);
    }
}