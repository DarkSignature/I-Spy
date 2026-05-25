using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteractable : MonoBehaviour
{
    protected Outline outline;

    protected virtual void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public virtual void OnGazeEnter()
    {
        outline.enabled = true;
    }

    public virtual void OnGazeExit()
    {
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

