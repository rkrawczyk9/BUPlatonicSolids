using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeOld : IDeletable
{
    public List<GameObject> connectedNodes = new List<GameObject>();
    public List<GameObject> connectedEdges = new List<GameObject>();
    public List<GameObject> connectedFaces = new List<GameObject>();
    public GameObject edgePrefab;

    public bool SetEdges()
    {
        // Spawn and initialize each 
        foreach(GameObject obj in connectedNodes)
        {
            // Check if this node already exists in the other node
            List<GameObject> otherEdges = obj.GetComponent<NodeOld>().connectedEdges;
            bool isFound = false;
            foreach(GameObject edge in otherEdges)
            {
                if(edge.GetComponent<EdgeScript>().CheckOtherNode(this.gameObject))
                {
                    isFound = true;
                    break;
                }
            }
            
            if(!isFound)
            {
                // If not found, spawn a new edge
                GameObject latestEdge = Instantiate(edgePrefab);
                latestEdge.GetComponent<EdgeScript>().AssignNodes(this.gameObject, obj);
                latestEdge.GetComponent<EdgeScript>().Align();
                connectedEdges.Add(latestEdge);
                obj.GetComponent<NodeOld>().AddEdge(latestEdge);
            }
            
        }

        return false;
    }

    /// <summary>
    /// Add node to connect to
    /// </summary>
    /// <param name="newNode"></param>
    public void AddNode(GameObject newNode)
    {
        if(newNode != this.gameObject && !connectedNodes.Contains(newNode))
        {
            connectedNodes.Add(newNode);
        }
    }

    /// <summary>
    /// Checks if the passed node has this node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool HasNode(GameObject node)
    {
        return connectedNodes.Contains(node);
    }

    /// <summary>
    /// Add a new edge to its list
    /// </summary>
    /// <param name="newEdge"></param>
    public void AddEdge(GameObject newEdge)
    {
        connectedEdges.Add(newEdge);
    }

    public void AddFace(GameObject newFace)
    {
        connectedFaces.Add(newFace);
    }

    /// <summary>
    /// Delete this node and its references
    /// </summary>
    public override void Delete()
    {
        GameObject[] edges = connectedEdges.ToArray();
        GameObject[] face = connectedFaces.ToArray();

        for (int i = 0; i < edges.Length; i++)
        {
            edges[i].GetComponent<EdgeScript>().Delete();
        }
        for(int i = 0; i < face.Length; i++)
        {
            face[i].GetComponent<EdgeScript>().Delete();
        }

        Debug.Log("Calling self delete");
        // Delete this node
        Destroy(this.gameObject);
    }
    
}
