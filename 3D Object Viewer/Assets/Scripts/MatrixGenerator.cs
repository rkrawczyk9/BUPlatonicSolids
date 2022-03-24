using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MatrixGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> allNodes = new List<GameObject>();
    public string matrix;

    private void FindNodes()
    {
        GameObject[] temp = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in temp)
        {
            if (obj.CompareTag("Node"))
            {
                allNodes.Add(obj.gameObject);
            }
        }
    }

    public void AjacencyMatrix()
    {
        allNodes.Clear();

        FindNodes();

        for (int r = 0; r < allNodes.Count; r++)
        {
            for (int c = 0; c < allNodes.Count; c++)
            {
                Debug.Log("Row: " + allNodes[r]);
                Debug.Log("Column: " + allNodes[c]);

                if (allNodes[r].GetComponent<Node>().HasNode(allNodes[c]))
                {
                    matrix += "1";
                }
                else
                {
                    matrix += "0";
                }

                // TODO : Check for last object
                if(c+1 < allNodes.Count)
                    matrix += ",";
            }
            
            if(r+1 < allNodes.Count)
                matrix += "\n";
        }

        ExportMatrix();
    }

    public void ExportMatrix()
    {
        string path = Application.dataPath + "/Matrix.txt";
        string path2 = Application.dataPath + "/JSON Test.json";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, matrix);
            File.WriteAllText(path2, matrix);

        }
        else
        {
            File.Delete(path);
            File.WriteAllText(path, matrix);

            File.Delete(path2);
            File.WriteAllText(path2, matrix);
        }

        Debug.Log("New Matrix Saved!");
    }
}
