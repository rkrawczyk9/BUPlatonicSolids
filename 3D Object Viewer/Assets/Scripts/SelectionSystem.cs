using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionSystem : MonoBehaviour
{
    public List<GameObject> selectedObjects = new List<GameObject>();

    public GameObject facePrefab;

    public GameplayControls controller;

    public Material defaultMaterial;
    public Material selectedMaterial;

    InputAction leftCLick;
    InputAction rightClick;
    InputAction mousePos;
    InputAction shift;
    InputAction link;
    InputAction escape;
    InputAction f;

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
        escape = controller.Player.Escape;
        f = controller.Player.F;

        shift.started += ctx => multiSelecting = true;
        shift.canceled += ctx => multiSelecting = false;

        // link.performed += ctx => LinkEdges();
        link.performed += ctx => SpawnFace();

        leftCLick.performed += ctx => TrySelect();
        rightClick.performed += ctx => TryDelete();
        escape.performed += ctx => ClearSelection();
        f.performed += ctx => SelectNode();

        leftCLick.Enable();
        rightClick.Enable();
        mousePos.Enable();
        shift.Enable();
        link.Enable();
        escape.Enable();
        f.Enable();
    }

    public Vector2 MousePos
    {
        get
        {
            return mousePos.ReadValue<Vector2>();
        }
    }

    public void TrySelect()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);
        if (!multiSelecting)
        {
            ClearSelection();
        }

        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                hitInfo.collider.gameObject.GetComponent<Renderer>().material = selectedMaterial;

                if (!selectedObjects.Contains(hitInfo.collider.gameObject))
                    selectedObjects.Add(hitInfo.collider.gameObject);
            }
        }
    }

    public void ClearSelection()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.GetComponent<Renderer>().material = defaultMaterial;
        }
        selectedObjects.Clear();
    }

    public void TryDelete()
    {
        Debug.Log("Calling delete");
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);

        if(Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                Debug.Log("Hit something!");

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
    public void SpawnFace()
    {
        GameObject newFace = Instantiate(facePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        newFace.GetComponent<Face>().SpawnFace(selectedObjects);
    }

    /// <summary>
    /// Select the one main node
    /// </summary>
    public void SelectNode()
    {
        //Debug.Log("Selecting node!");
        if(selectedObjects.Count > 0 && selectedObjects[0].CompareTag("Node"))
        {
            selectedObjects[0].GetComponent<Node>().ClaimNode();
        }
    }

    #region Old 'Edge' System
    /*
    public void LinkNodes()
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
    }

    public void LinkEdges()
    {
        LinkNodes();

        NodeOld temp1;

        foreach (GameObject node in selectedObjects)
        {
            if (node.TryGetComponent<NodeOld>(out temp1))
            {
                temp1.SetEdges();
            }
        }
    }
    */
    #endregion

}
