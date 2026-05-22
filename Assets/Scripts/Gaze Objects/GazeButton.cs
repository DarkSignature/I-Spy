using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeButton : GazeInteractable
{
    private Outline outline;
    private Button button;

    void Start()
    {
        outline = GetComponent<Outline>();
        button = GetComponent<Button>();

        outline.enabled = false;
    }

    public override void OnGazeEnter()
    {
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
