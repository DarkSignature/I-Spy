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
        // Disabled buttons don't react to gaze at all
        if (button != null && !button.interactable)
            return;

        outline.enabled = true;
    }

    public override void OnGazeExit()
    {
        outline.enabled = false;
    }

    public override void OnGazeComplete()
    {
        // onClick.Invoke() bypasses interactable, so guard it here
        if (button != null && !button.interactable)
            return;

        button.onClick.Invoke();
    }

    public override float GetGazeTime()
    {
        return 2f;
    }
}
