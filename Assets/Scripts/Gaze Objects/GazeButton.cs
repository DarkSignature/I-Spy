using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeButton : GazeInteractable
{

    private Button button;

    protected override void Start()
    {
        base.Start();
        button = GetComponent<Button>();

        outline.enabled = false;
    }

    public override void OnGazeEnter()
    {
        Debug.Log("Gaze Entered");
        outline.enabled = true;
    }

    public override void OnGazeExit()
    {
        outline.enabled = false;
    }

    public override void OnGazeComplete()
    {
        button.onClick.Invoke();
    }

    public override float GetGazeTime()
    {
        return 2f;
    }
}
