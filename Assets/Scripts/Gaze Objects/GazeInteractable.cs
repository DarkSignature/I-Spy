using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteractable : MonoBehaviour
{
    private Outline outline;

    void Start()
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

