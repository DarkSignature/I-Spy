using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{

    public TMP_Text scoreboard;
    public int round = 0;

    Dictionary<int, int> scores = new Dictionary<int, int>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnJoinedRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!scores.ContainsKey(player.ActorNumber))
            {
                scores.Add(player.ActorNumber, 0);
            }
        }

        UpdateScoreboard();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (scores.ContainsKey(otherPlayer.ActorNumber))
        {
            scores.Remove(otherPlayer.ActorNumber);
        }

        UpdateScoreboard();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!scores.ContainsKey(newPlayer.ActorNumber))
        {
            scores.Add(newPlayer.ActorNumber, 0);
        }

        UpdateScoreboard();
    }

    void UpdateScoreboard()
    {
        scoreboard.text = "Ranking\n";

        foreach (var entry in scores)
        {
            int actorNumber = entry.Key;
            int score = entry.Value;

            if (PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumber, out Player player))
            {
                if(player.NickName == "")
                {
                    player.NickName = "Player " + actorNumber;
                }
                scoreboard.text += player.NickName + " - " + score + "\n";
            }
        }
    }
}
