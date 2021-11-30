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

    public void GenerateAdjacencyMatrix()
    {
        FindNodes();

        //matrix += "\t";

        //foreach (GameObject namineNode in allNodes)
        //{
        //    //matrix += namineNode.name + ":\t";
        //}

       // matrix += "\n";

        foreach (GameObject rowNode in allNodes)
        {
            //matrix += rowNode.name + ":\t";

            foreach (GameObject columnNode in allNodes)
            {
                if (rowNode.GetComponent<Node>().HasNode(columnNode))
                {
                    matrix += "1";
                }
                else
                {
                    matrix += "0";
                }
                matrix += "\t";
            }
            //matrix += "\n";
        }

        ExportMatrix();
    }


    public void ExportMatrix()
    {
        string path = Application.dataPath + "/Matrix.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, matrix);
        }
        else
        {
            File.Delete(path);
            File.WriteAllText(path, matrix);
        }

        Debug.Log("New Matrix Saved!");
    }
}
