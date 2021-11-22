using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject spawnPos;

    public void SpawnNode()
    {
        Debug.Log("Spawning Node");
        Instantiate(node, spawnPos.transform.position, node.transform.rotation);
    }
}
