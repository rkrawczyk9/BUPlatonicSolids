using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<GameObject> connectedNodes = new List<GameObject>();
    public List<GameObject> connectedEdges = new List<GameObject>();
    public GameObject edgePrefab;

    private void Start()
    {
        // destroy if no linked nodes
        if(connectedNodes.Count == 0)
        {
            //Destroy(this);
        }
    }

    public bool SetEdges()
    {
        // Spawn and initialize each 
        foreach(GameObject obj in connectedNodes)
        {
            // Check if this node already exists in the other node
            List<GameObject> otherEdges = obj.GetComponent<Node>().connectedEdges;
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
                obj.GetComponent<Node>().AddEdge(latestEdge);
            }
            
        }

        return false;
    }

    public void AddNode(GameObject newNode)
    {
        if(newNode != this.gameObject && !connectedNodes.Contains(newNode))
        {
            connectedNodes.Add(newNode);
        }
    }

    public bool HasNode(GameObject node)
    {
        return connectedNodes.Contains(node);
    }

    public void AddEdge(GameObject newEdge)
    {
        connectedEdges.Add(newEdge);
    }

    
}
