using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Tracks per-player scores using Photon Player Custom Properties.
/// Each player writes their own score; other clients receive updates through
/// OnPlayerPropertiesUpdate, which is more reliable than RPCs (buffered,
/// survives late joins, and Photon guarantees delivery).
/// </summary>
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance;

    public const string ScoreKey = "score";

    /// <summary>Fired whenever any player's score changes; UI can listen here.</summary>
    public event System.Action OnScoresChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>Called by every client at the end of a round to record their own result.</summary>
    public void AwardPointsIfCorrect(bool wasCorrect, int pointsPerCorrect = 1)
    {
        if (!PhotonNetwork.InRoom || !wasCorrect)
            return;

        int current = GetScore(PhotonNetwork.LocalPlayer);
        SetScore(PhotonNetwork.LocalPlayer, current + pointsPerCorrect);
    }

    public int GetScore(Player player)
    {
        if (player == null || player.CustomProperties == null)
            return 0;

        return player.CustomProperties.TryGetValue(ScoreKey, out object raw) && raw is int value
            ? value
            : 0;
    }

    private void SetScore(Player player, int score)
    {
        Hashtable props = new Hashtable { { ScoreKey, score } };
        player.SetCustomProperties(props);
    }

    /// <summary>Returns players sorted by score (highest first). Ties keep join order.</summary>
    public List<Player> GetLeaderboard()
    {
        return PhotonNetwork.PlayerList
            .OrderByDescending(p => GetScore(p))
            .ThenBy(p => p.ActorNumber)
            .ToList();
    }

    /// <summary>Admin calls this at game start to wipe scores from any previous match.</summary>
    public void ResetAllScores()
    {
        if (!MainNetwork.IsAdmin)
            return;

        photonView.RPC(nameof(RPC_ResetLocalScore), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ResetLocalScore()
    {
        SetScore(PhotonNetwork.LocalPlayer, 0);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ScoreKey))
            OnScoresChanged?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnScoresChanged?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnScoresChanged?.Invoke();
    }
}
