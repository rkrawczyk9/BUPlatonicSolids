using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IDeletable
{
    public List<GameObject> linkedNodes = new List<GameObject>();
    public List<GameObject> linkedFaces = new List<GameObject>();
    //public List<GameObject> connectedNodes = new List<GameObject>();
    //public List<GameObject> connectedEdges = new List<GameObject>();
    //public GameObject edgePrefab;
    public uint id;

    public bool claimed = false;
    public Color claimedColor;
    private MeshRenderer ren;

    public void AddNode(GameObject newNode)
    {
        if (!linkedNodes.Contains(newNode))
            linkedNodes.Add(newNode);
    }
    public void AddFace(GameObject newFace)
    {
        if (!linkedFaces.Contains(newFace))
            linkedFaces.Add(newFace);
    }

    public bool HasNode(GameObject node)
    {
        if (linkedNodes.Count > 0 && linkedNodes.Contains(node))
        {
            return true;
        }
        return false;
    }

    public void RemoveNode(GameObject node)
    {
        if (linkedNodes.Count > 0 && linkedNodes.Contains(node))
        {
            linkedNodes.Remove(node);
        }
    }

    public void RemoveFace(GameObject face)
    {
        if (linkedFaces.Count > 0 && linkedFaces.Contains(face))
        {
            linkedFaces.Remove(face);
        }
    }

    /// <summary>
    /// Check whether or not this and the passed node share any face
    /// </summary>
    /// <param name="node">The node being checked</param>
    public void IsLinked(GameObject node)
    {
        if (linkedNodes.Count <= 0)
            return;

        bool hasNode = false;
        foreach(GameObject face in linkedFaces)
        {
            if (face.GetComponent<Face>().GetNodes().Contains(node))
            {
                hasNode = true;
                break;
            }
        }

        if(!hasNode)
        {
            RemoveNode(node);
        }
    }

    /// <summary>
    /// Toggle the claimed status
    /// </summary>
    public void ClaimNode()
    {
        claimed = !claimed;
    }

    public bool IsClaimed()
    {
        return claimed;
    }

    public override void Delete()
    {
        GameObject[] faces = linkedFaces.ToArray();

        // Delete all faces this is a part of. Face Delete() should handle the node references
        for(int i = 0; i < faces.Length; i++)
        {
            faces[i].GetComponent<Face>().Delete();
        }

        Destroy(this.gameObject);
    }

    private void Start()
    {
        ren = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        if (claimed && ren != null && ren.material.color != claimedColor)
        {
            ren.material.color = claimedColor;
        }
    }
}