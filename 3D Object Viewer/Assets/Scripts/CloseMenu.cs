using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CloseMenu : MonoBehaviour
{
    public GameplayControls controller;

    InputAction escape;

    // Start is called before the first frame update
    void Awake()
    {
        controller = new GameplayControls();

        escape = controller.Player.Escape;
        escape.performed += ctx => Close();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        escape.Enable();
    }

    private void OnDisable()
    {
        escape.Disable();
    }
}
