using UnityEngine;
using UnityEngine.UI;

public class VRGazeInteractor : MonoBehaviour
{
    public Image progressImage;
    public RectTransform reticleTransform;
    public Canvas reticleCanvas;
    public Camera viewCamera;

    [Tooltip("Layers the gaze ray can hit. Include the 'Animal' layer plus any UI/button layers.")]
    public LayerMask gazeLayerMask = ~0;
    public float maxGazeDistance = 100f;

    private GazeInteractable currentInteractable;

    private float gazeTimer = 0f;

    private void Awake()
    {
        if (progressImage != null && reticleTransform == null)
        {
            reticleTransform = progressImage.rectTransform;
        }

        if (reticleCanvas == null)
        {
            reticleCanvas = GetComponentInParent<Canvas>();
        }

        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }

        CenterReticle();
    }

    void Update()
    {
        CenterReticle();

        Ray ray =
            new Ray(transform.position, transform.forward);
        
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, maxGazeDistance, gazeLayerMask))
        {
            GazeInteractable interactable =
                hit.collider.GetComponentInParent<GazeInteractable>();

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

    private void CenterReticle()
    {
        if (reticleTransform == null)
        {
            return;
        }

        if (reticleCanvas != null)
        {
            reticleTransform.anchoredPosition = Vector2.zero;
            reticleTransform.localPosition = Vector3.zero;
        }
        else if (viewCamera != null)
        {
            reticleTransform.position = viewCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));
        }
    }
}