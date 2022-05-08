using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionSystem : MonoBehaviour
{
    [Tooltip("Face prefab to spawn")]
    [SerializeField] private GameObject facePrefab;
    [Tooltip("Material to use when an object is selected")]
    [SerializeField] private Material selectedMaterial;
    [SerializeField] SolutionChecker solutionChecker;
    [SerializeField] LayerMask ignoreWhileTransparent = 1 << 6;
    [SerializeField] LayerMask ignoreWhileNotTransparent = 0;

    /// <summary>
    /// all currently selected gameobjects
    /// </summary>
    private List<GameObject> selectedObjects = new List<GameObject>();
    
    /// <summary>
    /// Input control variables
    /// </summary>
    private GameplayControls controller;
    private InputAction leftCLick;
    private InputAction rightClick;
    private InputAction mousePos;
    private InputAction shift;
    private InputAction link;
    private InputAction escape;
    private InputAction clear;

    /// <summary>
    /// Whether or not shift is being pressed
    /// </summary>
    private bool shiftMod = false;

    /// <summary>
    /// Prepare input system
    /// </summary>
    void Start()
    {
        controller = new GameplayControls();

        mousePos = controller.Player.MousePos;
        shift = controller.Player.Shift;
        leftCLick = controller.Player.Select;
        rightClick = controller.Player.RightClick;
        link = controller.Player.Link;
        escape = controller.Player.Escape;
        clear = controller.Player.Clear;

        shift.started += ctx => shiftMod = true;
        shift.canceled += ctx => shiftMod = false;

        link.performed += ctx => SpawnFace();
        leftCLick.performed += ctx => TrySelect();
        rightClick.performed += ctx => RightClick();
        escape.performed += ctx => ClearSelection();
        clear.performed += ctx => ResetAll();

        leftCLick.Enable();
        rightClick.Enable();
        mousePos.Enable();
        shift.Enable();
        link.Enable();
        escape.Enable();
        clear.Enable();
    }

    /// <summary>
    /// public getter for mouse position
    /// </summary>
    public Vector2 MousePos
    {
        get
        {
            return mousePos.ReadValue<Vector2>();
        }
    }

    /// <summary>
    /// Try selecting a game object
    /// </summary>
    public void TrySelect()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);
        if (!shiftMod)
        {
            ClearSelection();
        }

        // Decide if we can select through faces
        bool xray = solutionChecker.transparencyOn;
        LayerMask currLayerMask = xray ? ignoreWhileTransparent : ignoreWhileNotTransparent;
        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))//, currLayerMask))
        {
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                GameObject obj = hitInfo.collider.gameObject;
                if (!selectedObjects.Contains(obj))
                    selectedObjects.Add(obj);

                // Tell selected object its selected and to stop updating its color, color selected
                obj.GetComponent<IDeletable>().Selected(true);
                obj.GetComponent<Renderer>().material = selectedMaterial;
            }
        }
    }

    /// <summary>
    /// Clear the selection of objects
    /// </summary>
    public void ClearSelection()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.GetComponent<IDeletable>().Selected(false);
        }
        selectedObjects.Clear();
    }

    /// <summary>
    /// Perform the right click action
    /// </summary>
    public void RightClick()
    {
        // Toggle node claim if not shift, delete if shift. 
        if(!shiftMod)
        {
            TrySelect();
            ClaimNode();
            ClearSelection();
        }
        else
        {
            TryDelete();
        }
    }

    /// <summary>
    /// Claim the targeted node
    /// </summary>
    public void ClaimNode()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);

        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                Node temp;
                if (hitInfo.collider.TryGetComponent<Node>(out temp))
                {
                    temp.ClaimNode();
                }
            }
        }
    }

    /// <summary>
    /// Try deleting the hit node or face
    /// </summary>
    public void TryDelete()
    {
        RaycastHit hitInfo;
        Ray rayOrigin = Camera.main.ScreenPointToRay(MousePos);

        if(Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            // Add hit object to list
            if (hitInfo.collider.gameObject != null)
            {
                IDeletable temp;
                if(hitInfo.collider.TryGetComponent<IDeletable>(out temp))
                {
                    temp.Delete();
                }

                selectedObjects.Clear();
            }
        }
    }

    /// <summary>
    /// Delete every node in scene, reset ID system
    /// </summary>
    public void ResetAll()
    {
        Node[] temp = FindObjectsOfType<Node>();

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].Delete();
        }

        FindObjectOfType<SpawnManager>().ResetID();
    }

    /// <summary>
    /// Spawn in the face between selected nodes. Used by UI
    /// </summary>
    public void SpawnFace()
    {
        GameObject newFace = Instantiate(facePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), null);
        newFace.GetComponent<Face>().SpawnFace(selectedObjects);
    }

    /// <summary>
    /// Spawn in the face between selected nodes. Used by autobuilder
    /// </summary>
    /// <param name="nodes">All nodes to be use when making the face</param>
    public void SpawnFace(List<GameObject> nodes, Transform parent = null)
    {
        GameObject newFace = Instantiate(facePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), parent);
        newFace.GetComponent<Face>().SpawnFace(nodes);
    }
}
