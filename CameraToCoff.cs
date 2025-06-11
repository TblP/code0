using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToCoff : MonoBehaviour
{
    public Transform cameraPosition; 
    public float transitionSpeed = 2f;
    private Camera mainCamera;
    private Vector3 originalPosition;
    public Transform LookAtPos;

    public bool state_ = false;
    private ControllerComp controllerPlayer;
    void Start()
    {
        mainCamera = Camera.main;
        controllerPlayer = GameObject.Find("Player").GetComponent<ControllerComp>();
    }
    private void Update()
    {
        if (state_)
        {
            mainCamera.transform.LookAt(LookAtPos);
            if (Input.GetKey(KeyCode.Q))
            {
                controllerPlayer.MatorikaOn();
                EndInteraction();
            }
        }

    }
    public void StartInteraction()
    {
        // —охран€ем исходное положение камеры
        originalPosition = mainCamera.transform.position;
        state_ = true;
        // Ќачинаем корутину дл€ плавного перемещени€
        StartCoroutine(MoveCameraToScreen());
        
    }
    public void EndInteraction()
    {
        state_ = false;
        // Ќачинаем корутину дл€ плавного перемещени€
        StartCoroutine(MoveCameraBack());
    }
    IEnumerator MoveCameraToScreen()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            // ѕлавное перемещение и поворот камеры
            mainCamera.transform.position = Vector3.Lerp(originalPosition,
                                                       cameraPosition.position,
                                                       elapsedTime);
            yield return null;
        }
    }
    IEnumerator MoveCameraBack()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            // ѕлавное перемещение и поворот камеры
            mainCamera.transform.position = Vector3.Lerp(cameraPosition.position,
                                                       originalPosition,
                                                       elapsedTime);

            yield return null;
        }
    }
}
