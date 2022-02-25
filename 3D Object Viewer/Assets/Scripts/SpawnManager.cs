using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Node node;
    [SerializeField] private GameObject spawnPos;

    uint curr_default_id = 1;

    public Node SpawnNode()
    {
        Debug.Log("Spawning Node");
        Node newNode = Instantiate(node, spawnPos.transform.position, node.transform.rotation);
        newNode.id = curr_default_id++;
        return newNode;
    }
}
