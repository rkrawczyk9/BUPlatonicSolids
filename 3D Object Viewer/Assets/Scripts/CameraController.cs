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

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float closeZoom;
    [SerializeField] private float farZoom;
    private float currentZoom;

    [SerializeField] GameObject cam;
    [SerializeField] GameObject pivot;

    private Vector3 currentLocation;
    private Vector3 currentRotation;
    private Vector3 currentTilt;

    GameplayControls controller;

    InputAction movement;
    InputAction pan;
    InputAction rising;
    InputAction rotateY;
    InputAction dragX;
    InputAction dragY;
    InputAction tilt;
    InputAction zoom;
    InputAction lmbDown;

    private bool isRotating = false;
    private bool isDraggingX = false;
    private bool isDraggingY = false;
    private bool isMoving = false;
    private bool isPanning = false;
    private bool isRising = false;
    private bool isTilting = false;
    private bool isZoooming = false;
    private bool isClicking = false;


    private void Start()
    {
        currentLocation = pivot.transform.position;
        currentRotation = pivot.transform.rotation.eulerAngles;
        currentTilt = cam.transform.rotation.eulerAngles;
        currentZoom = cam.transform.localPosition.z;

        controller = new GameplayControls();

        rotateY = controller.Player.Rotate;
        rotateY.started += ctx => isRotating = true;
        rotateY.canceled += ctx => isRotating = false;

        dragX = controller.Player.DragX;
        dragX.started += ctx => isDraggingX = true;
        dragX.canceled += ctx => isDraggingX = false;

        dragY = controller.Player.DragY;
        dragY.started += ctx => isDraggingY = true;
        dragY.canceled += ctx => isDraggingY = false;

        tilt = controller.Player.Tilt;
        tilt.started += ctx => isTilting = true;
        tilt.canceled += ctx => isTilting = false;

        movement = controller.Player.Move;
        movement.started += ctx => isMoving = true;
        movement.canceled += ctx => isMoving = false;

        pan = controller.Player.Pan;
        pan.started += ctx => isPanning = true;
        pan.canceled += ctx => isPanning = false;

        rising = controller.Player.Raise;
        rising.started += ctx => isRising = true;
        rising.canceled += ctx => isRising = false;

        zoom = controller.Player.Zoom;
        zoom.started += ctx => isZoooming = true;
        zoom.canceled += ctx => isZoooming = false;

        lmbDown = controller.Player.LMBDown;
        lmbDown.started += ctx => isClicking = true;
        lmbDown.canceled += ctx => isClicking = false;

        rotateY.Enable();
        dragX.Enable();
        dragY.Enable();
        tilt.Enable();
        movement.Enable();
        pan.Enable();
        rising.Enable();
        zoom.Enable();
        lmbDown.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRotating || (isClicking && (isDraggingX)))
        {
            RotateCamera();
        }
        if(isTilting || (isClicking && (isDraggingY)))
        {
            TiltCamera();
        }
        if(isMoving || isRising || (isPanning && (isDraggingX || isDraggingY)))
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
        float moveSide = movement.ReadValue<Vector2>().x + dragX.ReadValue<float>();
        float moveForward = movement.ReadValue<Vector2>().y;
        float moveUp = rising.ReadValue<float>() + dragY.ReadValue<float>();
        print($"side: {moveSide}\t\tforward: {moveForward}\t\tup: {moveUp}");

        // right and left
        pivot.transform.Translate(Vector3.right * moveSide * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
        currentLocation = pivot.transform.position;

        // back and forward
        pivot.transform.Translate(Vector3.forward * moveForward * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
        currentLocation = pivot.transform.position;

        // down and up
        pivot.transform.Translate(Vector3.up * moveUp * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
        currentLocation = pivot.transform.position;
    }

    private void RotateCamera()
    {
        float rotateAmount = rotateY.ReadValue<float>() + -dragX.ReadValue<float>();
        
        // rotating side to side
        pivot.transform.Rotate(Vector3.up, rotateAmount * rotateSpeed * rotateSpeedMultiplier * Time.deltaTime);
        currentRotation = pivot.transform.rotation.eulerAngles;
    }

    private void TiltCamera() // pitch
    {
        float tiltAmount = tilt.ReadValue<float>() + dragY.ReadValue<float>();

        // rotating downwards and upwards
        cam.transform.Rotate(Vector3.right, tiltAmount * tiltSpeed * Time.deltaTime);
        currentTilt = cam.transform.rotation.eulerAngles;
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
