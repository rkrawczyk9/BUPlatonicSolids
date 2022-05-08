using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : IDeletable
{
    [Tooltip("The ID for this node")]
    public uint id = 0;
    [Tooltip("List of nodes that this node is linked with")]
    [SerializeField] private List<GameObject> linkedNodes = new List<GameObject>();
    [Tooltip("List of faces that this node is linked with")]
    [SerializeField] private List<GameObject> linkedFaces = new List<GameObject>();
    // 4th dimension coord
    public float _w = 0;

    /// <summary>
    /// Whether this node is claimed and assosiated faces should be too
    /// </summary>
    private bool claimed = false;
    /// <summary>
    /// Mesh renderer for this object
    /// </summary>
    private MeshRenderer ren;
    /// <summary>
    /// Tags for this object 
    /// </summary>
    private TextMeshProUGUI[] tags;

    /// <summary>
    /// Initilization 
    /// </summary>
    private void Awake()
    {
        StartCoroutine(SetID());

        ren = GetComponent<MeshRenderer>();
        tags = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void SetW(float w)
    {
        _w = w;
    }

    /// <summary>
    /// Set the visual text objects to the ID when loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetID()
    {
        // Keep trying to set the text labels to ID until its successful
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (tags.Length != 0 && tags[0].text == "" && id != 0)
            {
                foreach (var txt in tags)
                {
                    txt.text = id.ToString();
                }
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Manage color updating
    /// </summary>
    private void FixedUpdate()
    {
        if (ren is null)
            return;

        if (!colorLocked && claimed && ren.material != claimedMaterial)
        {
            ren.material = claimedMaterial;
        }
        else if (!colorLocked && !claimed && ren.material != normalMaterial)
        {
            ren.material = normalMaterial;
        }
    }

    #region Node
    /// <summary>
    /// Toggle the claimed status
    /// </summary>
    public void ClaimNode()
    {
        claimed = !claimed;
    }
    /// <summary>
    /// public getter
    /// </summary>
    /// <returns></returns>
    public bool IsClaimed()
    {
        return claimed;
    }

    /// <summary>
    /// Add a node to this list
    /// </summary>
    /// <param name="newNode">Node to add</param>
    public void AddNode(GameObject newNode)
    {
        if (!linkedNodes.Contains(newNode))
            linkedNodes.Add(newNode);
    }
    /// <summary>
    /// Check to see if this and the other node are linked
    /// </summary>
    /// <param name="node">Node to check</param>
    /// <returns>The connection status between nodes</returns>
    public bool HasNode(GameObject node)
    {
        if (linkedNodes.Count > 0 && linkedNodes.Contains(node))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Remove node from this node's connection list
    /// </summary>
    /// <param name="node">Node to remove from connection list</param>
    public void RemoveNode(GameObject node)
    {
        if (linkedNodes.Count > 0 && linkedNodes.Contains(node))
        {
            linkedNodes.Remove(node);
        }
    }

    /// <summary>
    /// Check whether or not this and the passed node share any face. If not, removes the node
    /// </summary>
    /// <param name="node">The node being checked</param>
    public void IsLinked(GameObject node)
    {
        if (linkedNodes.Count <= 0)
            return;

        bool hasNode = false;
        foreach (GameObject face in linkedFaces)
        {
            if (face.GetComponent<Face>().GetNodes().Contains(node))
            {
                hasNode = true;
                break;
            }
        }

        if (!hasNode)
        {
            RemoveNode(node);
        }
    }
    #endregion

    #region Face
    /// <summary>
    /// Add face to this object
    /// </summary>
    /// <param name="newFace">new face to add</param>
    public void AddFace(GameObject newFace)
    {
        if (!linkedFaces.Contains(newFace))
            linkedFaces.Add(newFace);
    }
    /// <summary>
    /// Remove face from this node
    /// </summary>
    /// <param name="face">face to remove</param>
    public void RemoveFace(GameObject face)
    {
        if (linkedFaces.Count > 0 && linkedFaces.Contains(face))
        {
            linkedFaces.Remove(face);
        }
    }
    #endregion

    public override void Delete()
    {
        GameObject[] faces = linkedFaces.ToArray();

        // Delete all faces this is a part of. Face Delete() should handle the node references
        for(int i = 0; i < faces.Length; i++)
        {
            faces[i].GetComponent<Face>().Delete();
        }
        // Tell spawn manager that this ID can be reused
        FindObjectOfType<SpawnManager>().DeletedID(id);

        Destroy(this.gameObject);
    }

    /// <summary>
    /// If overlapping another node, delete itself
    /// </summary>
    /// <param name="collision">another object</param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Node"))
        {
            Debug.Log("Overlapping node detected");
            Delete();
        }
    }
}