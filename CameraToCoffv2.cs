using System.Collections;
using UnityEngine;

public class CameraToCoff : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraPosition; 
    public float transitionSpeed = 2f;
    public Transform lookAtPosition;

    [Header("References")]
    private Camera _mainCamera;
    private Vector3 _originalPosition;
    private ControllerComp _controllerPlayer;
    
    [Header("State")]
    public bool isInteracting = false;

    private void Start()
    {
        _mainCamera = Camera.main;
        _controllerPlayer = GameObject.Find("Player").GetComponent<ControllerComp>();
    }

    private void Update()
    {
        if (isInteracting)
        {
            _mainCamera.transform.LookAt(lookAtPosition);
            
            if (Input.GetKey(KeyCode.Q))
            {
                _controllerPlayer.MatorikaOn();
                EndInteraction();
            }
        }
    }

    public void StartInteraction()
    {
        _originalPosition = _mainCamera.transform.position;
        isInteracting = true;
        StartCoroutine(MoveCameraToScreenCoroutine());
    }

    public void EndInteraction()
    {
        isInteracting = false;
        StartCoroutine(MoveCameraBackCoroutine());
    }

    private IEnumerator MoveCameraToScreenCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            _mainCamera.transform.position = Vector3.Lerp(
                _originalPosition,
                cameraPosition.position,
                elapsedTime);
                
            yield return null;
        }
    }

    private IEnumerator MoveCameraBackCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            _mainCamera.transform.position = Vector3.Lerp(
                cameraPosition.position,
                _originalPosition,
                elapsedTime);

            yield return null;
        }
    }
}
