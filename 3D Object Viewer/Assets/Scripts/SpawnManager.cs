using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Tooltip("Prefab of the node to be spawned")]
    [SerializeField] private GameObject node;
    [Tooltip("The position reference point used for spawning object")]
    [SerializeField] private GameObject spawnPos;

    /// <summary>
    /// The current ID to use
    /// </summary>
    private uint curr_default_id = 1;

    /// <summary>
    /// Queue of ID's that are available to be used
    /// </summary>
    private Queue<uint> availableIDs = new Queue<uint>();

    /// <summary>
    /// Used by the autobuild script
    /// </summary>
    /// <returns></returns>
    public Node SpawnNode(Transform parent=null)
    {
        //Debug.Log("Spawning Node");
        Node newNode = Instantiate(node, spawnPos.transform.position, node.transform.rotation, parent).GetComponent<Node>();
        newNode.id = GetID();

        return newNode;
    }

    /// <summary>
    /// Used by the buttons on UI
    /// </summary>
    public void SpawnBuildNode()
    {
        Node newNode = Instantiate(node, spawnPos.transform.position, node.transform.rotation).GetComponent<Node>();
        newNode.id = GetID();
    }

    /// <summary>
    /// Get an id, either incremented or available
    /// </summary>
    /// <returns>ID to be used</returns>
    private uint GetID()
    {
        if (availableIDs.Count <= 0)
            return curr_default_id++;
        else
            return availableIDs.Dequeue();
    }

    /// <summary>
    /// Reset ID system
    /// </summary>
    public void ResetID()
    {
        availableIDs.Clear();
        curr_default_id = 1;
    }

    /// <summary>
    /// Add an ID to the ID recycle system
    /// </summary>
    /// <param name="id">ID to be recycled</param>
    public void DeletedID(uint id)
    {
        availableIDs.Enqueue(id);
    }

    public void Clear()
    {
        // Reload for now lol
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
