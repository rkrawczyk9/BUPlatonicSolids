using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeScript : IDeletable
{
    private GameObject[] nodes = new GameObject[2];

    public bool CheckOtherNode(GameObject srcNode)
    {
        if(nodes[0] == srcNode || nodes[1] == srcNode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AssignNodes(GameObject node1, GameObject node2)
    {
        nodes[0] = node1;
        nodes[1] = node2;

        node1.GetComponent<Node>().AddNode(node2);
        node2.GetComponent<Node>().AddNode(node1);
    }

    public void Align()
    {
        // Destroy unused objects
        if(nodes[0] == null || nodes[1] == null)
        {
            Destroy(this);
        }

        // place this object between the two objects
        float node1x, node1y, node1z, node2x, node2y, node2z;


        node1x = nodes[0].transform.position.x;
        node1y = nodes[0].transform.position.y;
        node1z = nodes[0].transform.position.z;

        node2x = nodes[1].transform.position.x;
        node2y = nodes[1].transform.position.y;
        node2z = nodes[1].transform.position.z;

        float xPos, yPos, zPos;

        xPos = ((node2x - node1x) / 2) + node1x;
        yPos = ((node2y - node1y) / 2) + node1y;
        zPos = ((node2z - node1z) / 2) + node1z;

        Vector3 newPos = new Vector3(xPos, yPos, zPos);
        transform.position = newPos;

        // Rotate the box to align between the objects
        gameObject.transform.LookAt(nodes[0].transform);

        // Stretch the box to replicate the line
        transform.localScale = new Vector3(.2f, .2f, Mathf.Abs(Vector3.Distance(nodes[0].transform.position, nodes[1].transform.position)));
    }

    public override void Delete()
    {
        // Remove left node's references to other node and edge
        nodes[0].GetComponent<NodeOld>().connectedEdges.Remove(this.gameObject);
        nodes[0].GetComponent<NodeOld>().connectedNodes.Remove(nodes[1]);

        // Remove right node's references to other node and edge
        nodes[1].GetComponent<NodeOld>().connectedEdges.Remove(this.gameObject);
        nodes[1].GetComponent<NodeOld>().connectedNodes.Remove(nodes[0]);

        // Destroy
        Destroy(this.gameObject);
    }
}
