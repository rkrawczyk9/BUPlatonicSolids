using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject spawnPos;

    uint curr_default_id = 1;

    /// <summary>
    /// Used by the autobuild script
    /// </summary>
    /// <returns></returns>
    public Node SpawnNode()
    {
        //Debug.Log("Spawning Node");
        Node newNode = Instantiate(node, spawnPos.transform.position, node.transform.rotation).GetComponent<Node>();
        newNode.id = curr_default_id++;
        return newNode;
    }

    /// <summary>
    /// Used by the buttons on UI
    /// </summary>
    public void SpawnBuildNode()
    {
        Node newNode = Instantiate(node, spawnPos.transform.position, node.transform.rotation).GetComponent<Node>();
        newNode.id = curr_default_id++;
    }
}
