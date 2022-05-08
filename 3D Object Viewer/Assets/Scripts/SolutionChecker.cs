using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolutionChecker : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

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
            uiManager.IsSolution();
        }
        else
        {
            Debug.Log("Not a solution!");
            uiManager.IsntSolution();
        }

        CheckNodes();
    }

    /// <summary>
    /// Check all faces to see if solution
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Check which nodes are currently claimed
    /// </summary>
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
        string sol = "Claimed Nodes: ";
        foreach(Node obj in claimedNodes)
        {
            sol += obj.id + ", ";
        }
        Debug.Log(sol);
    }
}
