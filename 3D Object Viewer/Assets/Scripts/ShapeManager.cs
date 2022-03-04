using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] List<GameObject> allNodes = new List<GameObject>();
    

    //void Start()
    //{
    //    GameObject[] temp = FindObjectsOfType<GameObject>();

    //    foreach(GameObject obj in temp)
    //    {
    //        if(obj.CompareTag("Node"))
    //        {
    //            allNodes.Add(obj.gameObject);
    //        }
    //    }

    //    GenerateShapes();
    //}

    //private void GenerateShapes()
    //{
    //    foreach(GameObject node in allNodes)
    //    {
    //        Debug.Log("Setting node " + node.name);
    //        node.GetComponent<NodeOld>().SetEdges();
    //    }
    //    //GenerateAdjacencyMatrix();
    //}
    
    
}
