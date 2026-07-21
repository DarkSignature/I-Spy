using UnityEngine;
using UnityEngine.UI;

public class VRGazeInteractor : MonoBehaviour
{
    public Image progressImage;

    private GazeInteractable currentInteractable;

    private float gazeTimer = 0f;

    void Update()
    {
        Ray ray =
            new Ray(transform.position, transform.forward);
        
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        {
            // Debug.Log(hit.collider.name);
            GazeInteractable interactable =
                hit.collider.GetComponent<GazeInteractable>();

            if(interactable != null)
            {
                HandleInteractable(interactable);
                return;
            }
        }

        ClearInteractable();
    }

    void HandleInteractable(
        GazeInteractable interactable)
    {
        if(interactable == currentInteractable)
        {
            gazeTimer += Time.deltaTime;
        }
        else
        {
            ClearInteractable();

            currentInteractable = interactable;

            currentInteractable.OnGazeEnter();

            gazeTimer = 0f;
        }

        progressImage.fillAmount =
            gazeTimer /
            currentInteractable.GetGazeTime();

        if(gazeTimer >=
            currentInteractable.GetGazeTime())
        {
            currentInteractable.OnGazeComplete();

            gazeTimer = 0f;

            progressImage.fillAmount = 0f;
        }
    }

    void ClearInteractable()
    {
        if(currentInteractable != null)
        {
            currentInteractable.OnGazeExit();

            currentInteractable = null;
        }

        gazeTimer = 0f;

        progressImage.fillAmount = 0f;
    }
}