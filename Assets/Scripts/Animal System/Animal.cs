using UnityEngine;

/// <summary>
/// Phase 4 - Animal component.
/// Attach this to every animal prefab inside Resources/Animals.
/// Extends GazeInteractable so the existing VRGazeInteractor
/// automatically handles gaze enter/exit/dwell on animals.
/// </summary>
public class Animal : GazeInteractable
{
    [Header("Identity")]
    public int animalID;
    public string animalName;

    [Header("Highlight Visuals")]
    [SerializeField] private Color gazeHighlightColor = Color.white;
    [SerializeField] private Color selectedHighlightColor = Color.yellow;
    [SerializeField] private Color correctRevealColor = Color.green;
    [SerializeField] private float outlineWidth = 6f;

    [Tooltip("Optional child object (e.g. a ring under the animal) shown while this animal is the player's current answer.")]
    [SerializeField] private GameObject selectionRingIndicator;

    /// <summary>Set by GameManager when this instance is spawned as the round's correct animal.</summary>
    public bool IsCorrectAnswer { get; set; }

    /// <summary>True while this animal is the player's currently tracked answer.</summary>
    public bool IsSelected { get; private set; }

    protected override void Start()
    {
        // Prefabs spawned from Resources may not carry an Outline component yet.
        if (GetComponent<Outline>() == null)
        {
            Outline added = gameObject.AddComponent<Outline>();
            added.OutlineMode = Outline.Mode.OutlineAll;
            added.OutlineWidth = outlineWidth;
        }

        base.Start();

        outline.OutlineColor = gazeHighlightColor;

        if (selectionRingIndicator != null)
            selectionRingIndicator.SetActive(false);
    }

    public override void OnGazeEnter()
    {
        RefreshHighlight(gazed: true);

        if (SelectionManager.Instance != null)
            SelectionManager.Instance.NotifyGazeEnter(this);
    }

    public override void OnGazeExit()
    {
        RefreshHighlight(gazed: false);

        if (SelectionManager.Instance != null)
            SelectionManager.Instance.NotifyGazeExit(this);
    }

    public override void OnGazeComplete()
    {
        // Dwell-select for VR gaze. Instant click/submit selection is
        // handled by SelectionManager.Update while this animal is gazed.
        if (SelectionManager.Instance != null)
            SelectionManager.Instance.SelectAnimal(this);
    }

    /// <summary>Marks/unmarks this animal as the player's current answer and updates visuals.</summary>
    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (selectionRingIndicator != null)
            selectionRingIndicator.SetActive(selected);

        RefreshHighlight(gazed: false);
    }

    /// <summary>Forces the "correct answer" highlight during the Reveal state.</summary>
    public void RevealAsCorrect()
    {
        if (outline == null)
            return;

        outline.OutlineColor = correctRevealColor;
        outline.enabled = true;
    }

    private void RefreshHighlight(bool gazed)
    {
        if (outline == null)
            return;

        if (IsSelected)
        {
            outline.OutlineColor = selectedHighlightColor;
            outline.enabled = true;
        }
        else
        {
            outline.OutlineColor = gazeHighlightColor;
            outline.enabled = gazed;
        }
    }
}
