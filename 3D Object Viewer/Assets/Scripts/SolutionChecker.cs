using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            ExportSet();
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
            sol += obj.id + ",";
        }
        Debug.Log(sol);
    }

    private void ExportSet()
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



        string path1 = Application.dataPath + "/Resources/SolutionSetID.txt";
        string path2 = Application.dataPath + "/Resources/SolutionSetCoord.txt";

        // get data
        string idSet = "";
        string coordSet = "";
        for(int i = 0; i < claimedNodes.Count; i++)
        {
            // set ID
            idSet += claimedNodes[i].id;
            
            Vector4 pos = claimedNodes[i].transform.position;
            // set coord
            coordSet += pos.x + "," + pos.y + "," + pos.z + "," + claimedNodes[i]._w;

            if (i + 1 < claimedNodes.Count)
            {
                idSet += ",";
                coordSet += "\n";
            }  
        }
        WriteFile(path1, idSet);
        WriteFile(path2, coordSet);
        Debug.Log("Exporting Done!");
    }

    private void WriteFile(string path, string data)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, data);
        }
        else
        {
            File.Delete(path);
            File.WriteAllText(path, data);
        }
        Debug.Log(path + " file saved!");
    }
}
