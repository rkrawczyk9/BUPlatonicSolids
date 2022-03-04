using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolutionChecker : MonoBehaviour
{
    private List<Face> faces;
    private List<Face> missingFaces;
    private List<Node> claimedNodes;

    private void Start()
    {
        faces = new List<Face>();
        missingFaces = new List<Face>();
        claimedNodes = new List<Node>();
    }

    public void CheckSolution()
    {
        if(CheckFaces())
        {
            Debug.Log("Solution!");
        }
        else
        {
            Debug.Log("Not a solution!");
        }

        CheckNodes();
    }

    private bool CheckFaces()
    {
        GameObject[] temp = FindObjectsOfType<GameObject>();

        faces.Clear();
        missingFaces.Clear();

        foreach (GameObject obj in temp)
        {
            if (obj.CompareTag("Face"))
            {
                faces.Add(obj.GetComponent<Face>());
            }
        }

        foreach(Face f in faces)
        {
            if (!f.claimed)
            {
                missingFaces.Add(f);
            }
        }

        return missingFaces.Count <= 0;
    }
    
    private void CheckNodes()
    {
        GameObject[] temp = FindObjectsOfType<GameObject>();

        claimedNodes.Clear();

        foreach (GameObject obj in temp)
        {
            if (obj.CompareTag("Node") && obj.GetComponent<Node>().claimed)
            {
                claimedNodes.Add(obj.GetComponent<Node>());
            }
        }

        Debug.Log("Currently claimed nodes:");
        foreach(Node node in claimedNodes)
        {
            Debug.Log(node.id);
        }
    }

}
