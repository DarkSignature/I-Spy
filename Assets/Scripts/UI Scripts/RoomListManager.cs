using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public int active_rooms = 0;
    public GameObject roomCardPrefab;
    public Transform contentParent;

    // Photon's OnRoomListUpdate sends DELTAS (only changed rooms), so we keep
    // a cache and update cards in place. This also means cards are no longer
    // destroyed and recreated every update, which was eating button clicks.
    private readonly Dictionary<string, RoomCard> roomCards = new();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            bool shouldRemove = room.RemovedFromList || !room.IsOpen || !room.IsVisible;

            if (shouldRemove)
            {
                if (roomCards.TryGetValue(room.Name, out RoomCard stale))
                {
                    if (stale != null)
                        Destroy(stale.gameObject);

                    roomCards.Remove(room.Name);
                }

                continue;
            }

            if (!roomCards.TryGetValue(room.Name, out RoomCard card) || card == null)
            {
                GameObject cardObject = Instantiate(roomCardPrefab, contentParent);
                cardObject.transform.localScale = Vector3.one;

                card = cardObject.GetComponent<RoomCard>();
                roomCards[room.Name] = card;

                Debug.Log("Created Room Card: " + room.Name);
            }

            card.Setup(room);
        }

        active_rooms = roomCards.Count;
        Debug.Log("Number of active_rooms: " + active_rooms);

        NotifyRoomAvailability();
    }

    /// <summary>
    /// Tells the main menu whether at least one joinable room (with a free
    /// slot) exists, so the player's join/play button can enable itself.
    /// </summary>
    private void NotifyRoomAvailability()
    {
        bool hasAvailableRoom = false;

        foreach (RoomCard card in roomCards.Values)
        {
            if (card != null && card.HasSpace)
            {
                hasAvailableRoom = true;
                break;
            }
        }

        if (MainRoomUI.Instance != null)
            MainRoomUI.Instance.SetRoomAvailability(hasAvailableRoom);
    }

    public override void OnJoinedLobby()
    {
        // The first update after joining a lobby is the FULL room list,
        // so wipe everything and let it rebuild from scratch.
        ClearRoomCards();
    }

    public override void OnLeftLobby()
    {
        ClearRoomCards();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ClearRoomCards();
    }

    private void ClearRoomCards()
    {
        foreach (RoomCard card in roomCards.Values)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        roomCards.Clear();
        active_rooms = 0;

        NotifyRoomAvailability();
    }
}
