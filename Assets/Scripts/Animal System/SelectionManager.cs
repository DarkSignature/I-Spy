using UnityEngine;

/// <summary>
/// Phase 5 - Selection system.
/// Tracks which animal the player is currently gazing at (fed by Animal via
/// the existing VRGazeInteractor) and stores the player's chosen answer.
/// The answer can be changed freely while GameManager is in the Answering state.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Input")]
    [Tooltip("Pressing this while gazing at an animal instantly selects it (dwell/gaze-complete also selects).")]
    [SerializeField] private KeyCode submitKey = KeyCode.Mouse0;

    private Animal currentGazedAnimal;
    private Animal trackedSelectedPlayerAnswer;

    /// <summary>The player's current answer for this round (null if none picked yet).</summary>
    public Animal TrackedSelectedPlayerAnswer => trackedSelectedPlayerAnswer;

    public bool HasAnswer => trackedSelectedPlayerAnswer != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (currentGazedAnimal != null && Input.GetKeyDown(submitKey))
            SelectAnimal(currentGazedAnimal);
    }

    public void NotifyGazeEnter(Animal animal)
    {
        currentGazedAnimal = animal;
    }

    public void NotifyGazeExit(Animal animal)
    {
        if (currentGazedAnimal == animal)
            currentGazedAnimal = null;
    }

    /// <summary>
    /// Stores the animal as the player's answer. Only works while the round
    /// timer is running (Answering state), so answers lock once time is up.
    /// </summary>
    public void SelectAnimal(Animal animal)
    {
        if (animal == null || !CanAnswer())
            return;

        if (animal == trackedSelectedPlayerAnswer)
            return;

        // Changing answer: clear the previous pick's visuals first.
        if (trackedSelectedPlayerAnswer != null)
            trackedSelectedPlayerAnswer.SetSelected(false);

        trackedSelectedPlayerAnswer = animal;
        trackedSelectedPlayerAnswer.SetSelected(true);

        Debug.Log($"Player answer set to: {animal.animalName} (ID {animal.animalID})");
    }

    public bool IsPlayerCorrect(Question question)
    {
        return question != null &&
               trackedSelectedPlayerAnswer != null &&
               trackedSelectedPlayerAnswer.animalID == question.correctAnimalID;
    }

    /// <summary>Called by GameManager when a round ends.</summary>
    public void ClearSelection()
    {
        if (trackedSelectedPlayerAnswer != null)
            trackedSelectedPlayerAnswer.SetSelected(false);

        trackedSelectedPlayerAnswer = null;
        currentGazedAnimal = null;
    }

    private bool CanAnswer()
    {
        return GameManager.Instance != null &&
               GameManager.Instance.CurrentState == GameState.Answering;
    }
}
