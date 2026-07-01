using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState;
    private Question currentQuestion;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartRound();
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
        Debug.Log("Showing Answer");

        Invoke(nameof(EndRound), 5f);
    }

    public void EndRound()
    {
        Debug.Log("Round End");

        StartRound();
    }
}