using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionSystem : MonoBehaviour
{
    public float range;
    public List<GameObject> selectedObjects = new List<GameObject>();

    public GameplayControls controller;

    InputAction leftCLick;

    // Start is called before the first frame update
    void Start()
    {
        controller = new GameplayControls();

        leftCLick = controller.Player.Select;
        leftCLick.started += ctx => TrySelect();

        leftCLick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TrySelect()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenToWorldPoint(co), Camera.main.transform.forward, out hitInfo, range))
        {
            Debug.Log(hitInfo.transform.gameObject.name);

            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                selectedObjects.Add(hitInfo.collider.gameObject);
            }
        }
    }
}
