using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedMultiplier;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateSpeedMultiplier;
    [SerializeField] private float tiltSpeed;
    [SerializeField] private Vector2 tiltBounds;

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float closeZoom;
    [SerializeField] private float farZoom;
    private float currentZoom;

    [SerializeField] GameObject cam;
    [SerializeField] GameObject pivot;

    private Vector3 currentLocation;
    private Vector3 currentRotation;
    private float currentTilt;

    GameplayControls controller;

    InputAction movement;
    InputAction rising;
    InputAction rotateY;
    InputAction tilt;
    InputAction zoom;

    private bool isRotating = false;
    private bool isMoving = false;
    private bool isRising = false;
    private bool isTilting = false;
    private bool isZoooming = false;

    private void Start()
    {
        currentLocation = pivot.transform.position;
        currentRotation = pivot.transform.rotation.eulerAngles;
        currentTilt = 0;
        currentZoom = cam.transform.localPosition.z;

        controller = new GameplayControls();

        rotateY = controller.Player.Rotate;
        rotateY.started += ctx => isRotating = true;
        rotateY.canceled += ctx => isRotating = false;

        tilt = controller.Player.Tilt;
        tilt.started += ctx => isTilting = true;
        tilt.canceled += ctx => isTilting = false;

        movement = controller.Player.Move;
        movement.started += ctx => isMoving = true;
        movement.canceled += ctx => isMoving = false;

        rising = controller.Player.Raise;
        rising.started += ctx => isRising = true;
        rising.canceled += ctx => isRising = false;

        zoom = controller.Player.Zoom;
        zoom.started += ctx => isZoooming = true;
        zoom.canceled += ctx => isZoooming = false;

        rotateY.Enable();
        tilt.Enable();
        movement.Enable();
        rising.Enable();
        zoom.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRotating)
        {
            RotateCamera();
        }
        if(isTilting)
        {
            TiltCamera();
        }
        if(isMoving || isRising)
        {
            MoveCamera();
        }
        if(isZoooming)
        {
            ZoomCamera();
        }
    }

    private void MoveCamera()
    {
        Vector2 direction = movement.ReadValue<Vector2>();
        float raise = rising.ReadValue<float>();

        // left and right
        if (direction.x < 0)
        {
            pivot.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }
        else if (direction.x > 0)
        {
            pivot.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }

        // back and forward
        if(direction.y < 0)
        {
            pivot.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }
        else if (direction.y > 0)
        {
            pivot.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }

        // down and up
        if(raise < 0)
        {
            pivot.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }
        else if (raise > 0)
        {
            pivot.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            currentLocation = pivot.transform.position;
        }
    }

    private void RotateCamera()
    {
        float direction = rotateY.ReadValue<float>();
        

        if (direction < 0)
        {
            pivot.transform.Rotate(Vector3.up, -rotateSpeed);
            currentRotation = pivot.transform.rotation.eulerAngles;
        }
        else if (direction > 0)
        {
            pivot.transform.Rotate(Vector3.up, rotateSpeed);
            currentRotation = pivot.transform.rotation.eulerAngles;
        }

        
    }

    private void TiltCamera()
    {
        float direction = tilt.ReadValue<float>();

        // Tilt upwards
        if (direction < 0 && currentTilt > tiltBounds.y)
        {
            cam.transform.Rotate(Vector3.right, -tiltSpeed);
            currentTilt -= tiltSpeed;
        }
        // Tilt downwards
        else if (direction > 0 && currentTilt < tiltBounds.x)
        {
            cam.transform.Rotate(Vector3.right, tiltSpeed);
            currentTilt += tiltSpeed;
        }
        
    }

    private void ZoomCamera()
    {
        Vector2 direction = zoom.ReadValue<Vector2>();

        if(direction.y < 0 && currentZoom < closeZoom)
        {
            cam.transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
            currentZoom = cam.transform.localPosition.z;
            if (currentZoom > closeZoom)
            {
                cam.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, closeZoom);
                currentZoom = cam.transform.localPosition.z;
            }
        }
        else if(direction.y > 0 && currentZoom > farZoom)
        {
            cam.transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
            currentZoom = cam.transform.localPosition.z;
            if (currentZoom < farZoom)
            {
                cam.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, farZoom);
                currentZoom = cam.transform.localPosition.z;
            }
        }
    }
}
