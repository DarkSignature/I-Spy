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
    public int currentQuestionID;
    public Question currentQuestion;
    private bool gameStarted = false;

    [Header("Animal Spawning (Phase 4)")]
    [Tooltip("Placement points in the map. Animals are shuffled across these each round.")]
    [SerializeField] private Transform[] spawnPoints;

    [Tooltip("How many wrong animals to spawn alongside the correct one.")]
    [SerializeField, Min(0)] private int distractorCount = 3;

    [Header("Answer Timer (Phase 6)")]
    [Tooltip("How long players have to answer, in seconds.")]
    [SerializeField, Min(1f)] private float answerDuration = 10f;

    /// <summary>Seconds left in the current Answering phase (0 when not answering).</summary>
    public float TimeRemaining { get; private set; }

    private Coroutine answerTimerRoutine;

    private GameObject[] animalPrefabs;
    private readonly List<Animal> spawnedAnimals = new();

    private void Awake()
    {
        Instance = this;

        // All animal prefabs must live in Assets/Resources/Animals
        // and carry an Animal component with a unique animalID.
        animalPrefabs = Resources.LoadAll<GameObject>("Animals");
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
                if (MainNetwork.IsAdmin)
                {
                    currentQuestionID = QuestionManager.Instance.GetRandomQuestionID();
                    MainNetwork.Instance.photonView.RPC("RPC_SetQuestion", RpcTarget.All, currentQuestionID);
                }
                break;

            case GameState.Answering:
                EnterAnswering();
                break;

            case GameState.Reveal:
                EnterReveal();
                currentQuestion = null;
                break;
        }
    }

    public void StartRound()
    {
        SetState(GameState.Animation);
    }

    public void EnterAnimation(Question currentQuestion)
    {
        Debug.Log("Playing Animation");

        SpawnAnimalsForRound(currentQuestion);

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

        StopAnswerTimer();
        answerTimerRoutine = StartCoroutine(AnswerTimerRoutine());
    }

    /// <summary>
    /// Phase 6 - Counts down the answer time and drives the countdown UI.
    /// While this runs, players can pick and change their answer.
    /// </summary>
    private IEnumerator AnswerTimerRoutine()
    {
        TimeRemaining = answerDuration;

        if (WaitingRoomUI.Instance != null)
            WaitingRoomUI.Instance.ShowTimer(answerDuration);

        while (TimeRemaining > 0f)
        {
            TimeRemaining -= Time.deltaTime;

            if (WaitingRoomUI.Instance != null)
                WaitingRoomUI.Instance.UpdateTimer(TimeRemaining, answerDuration);

            yield return null;
        }

        TimeRemaining = 0f;
        answerTimerRoutine = null;

        if (WaitingRoomUI.Instance != null)
            WaitingRoomUI.Instance.HideTimer();

        TimerFinished();
    }

    private void StopAnswerTimer()
    {
        if (answerTimerRoutine != null)
        {
            StopCoroutine(answerTimerRoutine);
            answerTimerRoutine = null;
        }

        TimeRemaining = 0f;

        if (WaitingRoomUI.Instance != null)
            WaitingRoomUI.Instance.HideTimer();
    }

    public void TimerFinished()
    {
        SetState(GameState.Reveal);
    }

    private void EnterReveal()
    {
        Debug.Log("Showing Answer - Waiting for admin to proceed");

        bool isCorrect = SelectionManager.Instance != null &&
                         SelectionManager.Instance.IsPlayerCorrect(currentQuestion);

        // Highlight the correct animal so every player sees the answer
        foreach (Animal animal in spawnedAnimals)
        {
            if (animal != null && animal.IsCorrectAnswer)
                animal.RevealAsCorrect();
        }

        // Show results panel
        if (WaitingRoomUI.Instance != null)
        {
            WaitingRoomUI.Instance.ShowResultsPanel(currentQuestion, isCorrect);
        }

        // Don't automatically end the round - wait for admin to click next
        // The admin will call AdminNextQuestion() which triggers EndRound()
    }

    public void EndRound()
    {
        Debug.Log("Round End");

        CleanupRound();

        StartRound();
    }

    /// <summary>
    /// Phase 4 - Spawns the correct animal plus random distractors,
    /// shuffled across the map's spawn points.
    /// </summary>
    private void SpawnAnimalsForRound(Question question)
    {
        DespawnAnimals();

        if (animalPrefabs == null || animalPrefabs.Length == 0)
        {
            Debug.LogError("No animal prefabs found in Resources/Animals!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned on GameManager!");
            return;
        }

        // 1. Split prefabs into the correct animal and distractor candidates
        GameObject correctPrefab = null;
        List<GameObject> distractorPool = new();

        foreach (GameObject prefab in animalPrefabs)
        {
            Animal animal = prefab.GetComponent<Animal>();

            if (animal == null)
            {
                Debug.LogWarning($"Prefab '{prefab.name}' in Resources/Animals has no Animal component - skipped.");
                continue;
            }

            if (correctPrefab == null && animal.animalID == question.correctAnimalID)
                correctPrefab = prefab;
            else
                distractorPool.Add(prefab);
        }

        if (correctPrefab == null)
        {
            Debug.LogError($"No animal prefab with animalID {question.correctAnimalID} for question '{question.questionText}'!");
            return;
        }

        // 2. Pick random distractors from the remaining prefabs
        Shuffle(distractorPool);

        int distractors = Mathf.Min(distractorCount, distractorPool.Count, spawnPoints.Length - 1);

        List<GameObject> toSpawn = new() { correctPrefab };
        toSpawn.AddRange(distractorPool.GetRange(0, distractors));

        // 3. Shuffle spawn points so the correct answer's position changes each round
        List<Transform> shuffledPoints = new(spawnPoints);
        Shuffle(shuffledPoints);

        for (int i = 0; i < toSpawn.Count; i++)
        {
            Transform point = shuffledPoints[i];
            GameObject instance = Instantiate(toSpawn[i], point.position, point.rotation);

            Animal animal = instance.GetComponent<Animal>();
            animal.IsCorrectAnswer = toSpawn[i] == correctPrefab;

            spawnedAnimals.Add(animal);
        }

        Debug.Log($"Spawned {toSpawn.Count} animals (correct ID: {question.correctAnimalID})");
    }

    private void DespawnAnimals()
    {
        foreach (Animal animal in spawnedAnimals)
        {
            if (animal != null)
                Destroy(animal.gameObject);
        }

        spawnedAnimals.Clear();
    }

    private void CleanupRound()
    {
        StopAnswerTimer();

        if (SelectionManager.Instance != null)
            SelectionManager.Instance.ClearSelection();

        DespawnAnimals();
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
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

        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_StartGame()
    {
        if (!gameStarted)
        {
            WaitingRoomUI.Instance.StartGameUI();
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

        photonView.RPC("RPC_NextQuestion", RpcTarget.All);
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
        photonView.RPC("RPC_EndGame", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_EndGame()
    {
        Debug.Log("Game Ended!");
        gameStarted = false;
        CancelInvoke(); // Cancel all pending invokes
        CleanupRound();
        // Return to waiting room - you can customize this behavior
        CurrentState = GameState.Waiting;
    }
}