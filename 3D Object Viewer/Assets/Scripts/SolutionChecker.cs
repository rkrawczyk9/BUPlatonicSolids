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
        Face[] temp = FindObjectsOfType<Face>();

        faces.Clear();
        missingFaces.Clear();

        foreach (Face obj in temp)
        {
            faces.Add(obj);
        }

        foreach(Face f in faces)
        {
            if (!f.Claimed)
            {
                missingFaces.Add(f);
            }
        }

        return missingFaces.Count <= 0;
    }
    
    public void ToggleFaceMat()
    {
        Face[] temp = FindObjectsOfType<Face>();
        Debug.Log("Faces found: " + temp.Length);

        foreach (Face obj in temp)
        {
            obj.ToggleTransparency();
        }

    }

    private void CheckNodes()
    {
        GameObject[] temp = FindObjectsOfType<GameObject>();

        claimedNodes.Clear();

        foreach (GameObject obj in temp)
        {
            if (obj.CompareTag("Node") && obj.GetComponent<Node>().IsClaimed())
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
