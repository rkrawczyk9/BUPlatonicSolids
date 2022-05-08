using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MatrixGenerator : MonoBehaviour
{
    /// <summary>
    /// All nodes detected in last scan
    /// </summary>
    private Node[] allNodes;
    /// <summary>
    /// The adjacency patrix
    /// </summary>
    private string matrix;

    /// <summary>
    /// Get all nodes in the scene
    /// </summary>
    private void FindNodes()
    {
        allNodes = FindObjectsOfType<Node>();
    }

    /// <summary>
    /// Create an adjacency matrix of all currently active nodes
    /// </summary>
    public void AjacencyMatrix()
    {
        FindNodes();
        matrix = "";

        for (int r = 0; r < allNodes.Length; r++)
        {
            for (int c = 0; c < allNodes.Length; c++)
            {
                //Debug.Log("Row: " + allNodes[r]);
                //Debug.Log("Column: " + allNodes[c]);

                if (allNodes[r].HasNode(allNodes[c].gameObject))
                {
                    matrix += "1";
                }
                else
                {
                    matrix += "0";
                }

                // TODO : Check for last object
                if(c+1 < allNodes.Length)
                    matrix += ",";
            }
            
            if(r+1 < allNodes.Length)
                matrix += "\n";
        }

        ExportMatrix();
    }

    /// <summary>
    /// Export the adjacency matrix to a txt file
    /// </summary>
    private void ExportMatrix()
    {
        string path = Application.dataPath + "/Matrix.txt";
        //string path2 = Application.dataPath + "/JSON Test.json";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, matrix);
            //File.WriteAllText(path2, matrix);

        }
        else
        {
            File.Delete(path);
            File.WriteAllText(path, matrix);

            //File.Delete(path2);
            //File.WriteAllText(path2, matrix);
        }

        Debug.Log("New Matrix Saved!");
    }
}
