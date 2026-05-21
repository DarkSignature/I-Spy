using UnityEngine;

public class GazeFunctions : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnGazeEnter()
    {
        outline.enabled = true;
    }

    public void OnGazeExit()
    {
        outline.enabled = false;
    }

    public void OnGazeComplete()
    {
        Debug.Log("Activated!");
    }
}