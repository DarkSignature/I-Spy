using System.Threading;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class VRGazeInteractor : MonoBehaviour
{
    public float gazeTime = 2f;
    public Image progressImage;

    private float gazeTimer = 0f;
    private NetworkCube currentCube;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // If raycast is triggered by an object with Collider
        {
            NetworkCube cube = hit.collider.GetComponent<NetworkCube>();

            if (cube != null) // Checks if the hit object is a cube
            {
                if (cube == currentCube) // If cube is the same cube as the previous update
                {
                    gazeTimer += Time.deltaTime; // Increase time gazing at cube
                }
                else // If cube is not the same as the previous update
                {
                    if(currentCube != null) 
                    {
                        currentCube.removeOutlineColor(); // Remove previous update cube
                    }
                    currentCube = cube; // Set new currentCube
                    gazeTimer = 0f; // Reset timer
                }

                cube.setOutlineColor(5); // Set outline colors of gazed cube
                if (gazeTimer >= gazeTime)
                {
                    cube.SelectCube(PhotonNetwork.LocalPlayer.ActorNumber);
                    gazeTimer = 0f;
                    progressImage.fillAmount = 0;
                }

                progressImage.fillAmount = gazeTimer / gazeTime; // Fill progress circle   

            }
        }
        else // If raycast is not triggered, meaning no objects are being gazed -> Remove Cube Outline + Reset
        {
            if(currentCube != null)
            {
                currentCube.removeOutlineColor();
            }
            currentCube = null;
            gazeTimer = 0;
            progressImage.fillAmount = 0;
        }

    }
}