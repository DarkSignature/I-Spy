using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteractable : MonoBehaviour
{
    protected Outline outline;

    [SerializeField]
    protected float outlineWidth = 10f;   // Adjust in Inspector

    protected virtual void Start()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.OutlineWidth = outlineWidth;
            outline.enabled = false;
        }
    }

    public virtual void OnGazeEnter()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public virtual void OnGazeExit()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public virtual void OnGazeComplete()
    {
        Debug.Log("Activated!");
    }

    public virtual float GetGazeTime()
    {
        return 2f;
    }
}