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
    public static GameManager Instance;

    public GameState CurrentState;
    private Question currentQuestion;
    private bool gameStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Game doesn't start automatically - only admin can start it
        // StartRound();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log("State Changed: " + newState);

        switch(newState)
        {
            case GameState.Animation:
                currentQuestion = QuestionManager.Instance.GetRandomQuestion();
                EnterAnimation(currentQuestion);
                break;

            case GameState.Answering:
                EnterAnswering();
                break;

            case GameState.Reveal:
                EnterReveal();
                break;
        }
    }

    public void StartRound()
    {
        SetState(GameState.Animation);
    }

    private void EnterAnimation(Question currentQuestion)
    {
        Debug.Log("Playing Animation");

        // Show animation UI
        WaitingRoomUI.Instance.ShowQuestion(currentQuestion);

        Invoke(nameof(AnimationFinished), 5f);
    }

    public void AnimationFinished()
    {
        SetState(GameState.Answering);
    }

    private void EnterAnswering()
    {
        Debug.Log("Players can answer now");

        Invoke(nameof(TimerFinished), 10f);
    }

    public void TimerFinished()
    {
        SetState(GameState.Reveal);
    }

    private void EnterReveal()
    {
        Debug.Log("Showing Answer - Waiting for admin to proceed");

        // Show results panel
        if (WaitingRoomUI.Instance != null)
        {
            WaitingRoomUI.Instance.ShowResultsPanel(currentQuestion, true);
        }

        // Don't automatically end the round - wait for admin to click next
        // The admin will call AdminNextQuestion() which triggers EndRound()
    }

    public void EndRound()
    {
        Debug.Log("Round End");

        StartRound();
    }

    /// <summary>
    /// Admin calls this to start the game for all players
    /// </summary>
    public void StartGameForAll()
    {
        if (!MainNetwork.IsAdmin)
        {
            Debug.LogError("Only admin can start the game!");
            return;
        }

        if (gameStarted)
        {
            Debug.LogWarning("Game already started!");
            return;
        }

        photonView.RPC("RPC_StartGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            Debug.Log("Game Started for all players!");
            StartRound();
        }
    }

    /// <summary>
    /// Admin calls this to proceed to next question
    /// </summary>
    public void AdminNextQuestion()
    {
        if (!MainNetwork.IsAdmin)
        {
            Debug.LogError("Only admin can proceed to next question!");
            return;
        }

        photonView.RPC("RPC_NextQuestion", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_NextQuestion()
    {
        // Called when admin clicks next on result screen
        // This will move to the next question
        CancelInvoke(nameof(EndRound)); // Cancel the automatic end round if still pending
        EndRound();
    }

    /// <summary>
    /// Admin calls this to end the game
    /// </summary>
    public void AdminEndGame()
    {
        if (!MainNetwork.IsAdmin)
        {
            Debug.LogError("Only admin can end the game!");
            return;
        }

        gameStarted = false;
        photonView.RPC("RPC_EndGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_EndGame()
    {
        Debug.Log("Game Ended!");
        gameStarted = false;
        CancelInvoke(); // Cancel all pending invokes
        // Return to waiting room - you can customize this behavior
        CurrentState = GameState.Waiting;
    }
}