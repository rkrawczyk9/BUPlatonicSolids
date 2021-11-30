using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionSystem : MonoBehaviour
{
    public List<GameObject> selectedObjects = new List<GameObject>();

    public GameplayControls controller;

    public Material defaultMaterial;
    public Material selectedMaterial;

    InputAction leftCLick;
    InputAction rightClick;
    InputAction mousePos;
    InputAction shift;
    InputAction link;

    bool multiSelecting = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = new GameplayControls();

        mousePos = controller.Player.MousePos;
        shift = controller.Player.Shift;
        leftCLick = controller.Player.Select;
        rightClick = controller.Player.Delete;
        link = controller.Player.Link;

        shift.started += ctx => multiSelecting = true;
        shift.canceled += ctx => multiSelecting = false;

        link.performed += ctx => LinkEdges();

        leftCLick.performed += ctx => TrySelect();
        rightClick.performed += ctx => TryDelete();

        leftCLick.Enable();
        rightClick.Enable();
        mousePos.Enable();
        shift.Enable();
        link.Enable();
    }

    public Vector2 MousePos
    {
        get
        {
            return mousePos.ReadValue<Vector2>();
        }
    }

    public void TryDelete()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);

        if(Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            //Debug.Log("Hit");
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                IDeletable temp;
                if(hitInfo.collider.TryGetComponent<IDeletable>(out temp))
                {
                    temp.Delete();
                }

                foreach (GameObject obj in selectedObjects)
                {
                    obj.GetComponent<Renderer>().material = defaultMaterial;
                }
                selectedObjects.Clear();
            }
        }
    }


    public void TrySelect()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);
        //Debug.Log("Shoot");
        if (!multiSelecting)
        {
            foreach(GameObject obj in selectedObjects)
            {
                obj.GetComponent<Renderer>().material = defaultMaterial;
            }
            selectedObjects.Clear();
        }
            

        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            //Debug.Log("Hit");
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                hitInfo.collider.gameObject.GetComponent<Renderer>().material = selectedMaterial;

                if (!selectedObjects.Contains(hitInfo.collider.gameObject))
                    selectedObjects.Add(hitInfo.collider.gameObject);
            }
        }
    }

    public void LinkEdges()
    {
        Node temp1;
        Node temp2;
        foreach (GameObject node in selectedObjects)
        {
            if (node.TryGetComponent<Node>(out temp1))
            {

                foreach (GameObject node2 in selectedObjects)
                {
                    if (node2.TryGetComponent<Node>(out temp2))
                    {

                        if (temp1 != temp2)
                        {
                            temp1.AddNode(temp2.gameObject);
                            temp2.AddNode(temp1.gameObject);
                        }

                    }
                }

            }
        }

        foreach (GameObject node in selectedObjects)
        {
            if (node.TryGetComponent<Node>(out temp1))
            {
                temp1.SetEdges();
            }
        }
    }
}
