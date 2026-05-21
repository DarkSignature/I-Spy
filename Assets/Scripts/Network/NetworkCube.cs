using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class NetworkCube : MonoBehaviourPun
{
    Renderer cubeRenderer;
    Outline outline;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void SelectCube(int actorNumber)
    {
        photonView.RPC("RPC_ChangeColor", RpcTarget.AllBuffered, actorNumber);
    }

    public Color getColorFromActor(int actorNumber)
    {
        switch (actorNumber % 4){
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.yellow;
            case 3: return Color.green;
        }
        return Color.black;
    }

    public void setOutlineColor(int actorNumber)
    {
        outline.enabled = true;
        outline.OutlineColor = Color.magenta;
        outline.OutlineWidth = 7;
    }

    public void removeOutlineColor()
    {
        outline.enabled = false;
    }

    [PunRPC]
    void RPC_ChangeColor(int actorNumber)
    {
        cubeRenderer.material.color = getColorFromActor(actorNumber);
    }
}