using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] List<GameObject> allNodes = new List<GameObject>();
    public string matrix;

    public Text matrixText;

    void Start()
    {
        GameObject[] temp = FindObjectsOfType<GameObject>();

        foreach(GameObject obj in temp)
        {
            if(obj.CompareTag("Node"))
            {
                allNodes.Add(obj.gameObject);
            }
        }

        GenerateShapes();
    }

    private void GenerateShapes()
    {
        foreach(GameObject node in allNodes)
        {
            Debug.Log("Setting node " + node.name);
            node.GetComponent<Node>().SetEdges();
        }
        GenerateAdjacencyMatrix();
    }
    
    private void GenerateAdjacencyMatrix()
    {
        matrix += "\t";

        foreach (GameObject namineNode in allNodes)
        {
            matrix += namineNode.name + ":\t";
        }

        matrix += "\n";

        foreach (GameObject rowNode in allNodes)
        {
            matrix += rowNode.name + ":\t";

            foreach(GameObject columnNode in allNodes)
            {
                if(rowNode.GetComponent<Node>().HasNode(columnNode))
                {
                    matrix += "1";
                }
                else
                {
                    matrix += "0";
                }
                matrix += "\t";
            }
            matrix += "\n";
        }

        matrixText.text = matrix;
    }


    public void ExportMatrix()
    {
        string path = Application.dataPath + "/Matrix.txt";

        if(!File.Exists(path))
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
