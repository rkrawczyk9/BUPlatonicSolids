using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : IDeletable
{
    public List<GameObject> linkedNodes = new List<GameObject>();


    public bool claimed = false;
    public Color claimedColor;
    private MeshRenderer ren;

    private void Awake()
    {
        ren = GetComponent<MeshRenderer>();
        StartCoroutine(UpdateColor());
    }

    /// <summary>
    /// Spawn in face, assign connected nodes
    /// </summary>
    /// <param name="selectedObjects">List of nodes this face connects to</param>
    public void SpawnFace(List<GameObject> selectedObjects)
    {
        if(selectedObjects.Count >= 3)
        {
            foreach(GameObject node in selectedObjects)
            {
                linkedNodes.Add(node);
            }
        }
        else
        {
            Debug.Log("Not enough nodes to be a face. Deleting self");
            Destroy(this.gameObject);
        }

        GenerateFace();
    }

    /// <summary>
    /// Links all nodes with face and eachother data wise
    /// </summary>
    public void GenerateFace()
    {
        foreach (GameObject node in linkedNodes)
        {
            node.GetComponent<Node>().AddFace(this.gameObject);
            node.GetComponent<Renderer>().material.color = Color.green;
        }

        int temp;
        for(int i = 1; i < linkedNodes.Count; i++)
        {
            temp = i - 1;
            linkedNodes[i].GetComponent<Node>().AddNode(linkedNodes[temp]);
            linkedNodes[temp].GetComponent<Node>().AddNode(linkedNodes[i]);
        }

        linkedNodes[0].GetComponent<Node>().AddNode(linkedNodes[linkedNodes.Count-1]);
        linkedNodes[linkedNodes.Count-1].GetComponent<Node>().AddNode(linkedNodes[0]);


        Vector3[] positions = new Vector3[linkedNodes.Count];

        for(int i  = 0; i < linkedNodes.Count; i++)
        {
            positions[i] = linkedNodes[i].transform.position;
        }

        GetComponent<LineRenderer>().positionCount = positions.Length;
        GetComponent<LineRenderer>().SetPositions(positions);

        CreateMesh();
    }

    /// <summary>
    /// Generate the visual mesh for the face objects with triangles
    /// </summary>
    public void CreateMesh()
    {
        // Each triangle needs 3 points, 3 UV's, and a single number. Each set of 3 nodes will create 2 triangles
        Vector3[] points = new Vector3[(linkedNodes.Count - 2) * 6];
        Vector2[] uv = new Vector2[(linkedNodes.Count - 2) * 6];
        int[] triangles = new int[(linkedNodes.Count - 2) * 6];

        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        // Current node always starts with the third node as 3 atleast are required.
        int curNode = 2;
        int step = 0;
        do
        {
            // Every triangle will always connect to the core, node 0
            points[step] = points[step+5] = linkedNodes[0].transform.position;
            points[step+1] = points[step+4] = linkedNodes[curNode].transform.position;
            points[step+2] = points[step+3] = linkedNodes[curNode - 1].transform.position;

            uv[step] = uv[step+5] = new Vector2(linkedNodes[0].transform.position.x, linkedNodes[0].transform.position.z);
            uv[step + 1] = uv[step + 4] = new Vector2(linkedNodes[curNode].transform.position.x, linkedNodes[curNode].transform.position.z);
            uv[step + 2] = uv[step + 3] = new Vector2(linkedNodes[curNode - 1].transform.position.x, linkedNodes[curNode - 1].transform.position.z);

            curNode++;
            step += 6;
        } 
        while (curNode < linkedNodes.Count);

        Mesh mesh = new Mesh();
        
        mesh.Clear();

        mesh.vertices = points;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        ren = GetComponent<MeshRenderer>();
    }

    
    public List<GameObject> GetNodes()
    {
        return linkedNodes;
    }

    public override void Delete()
    {
        StopAllCoroutines();

        GameObject[] nodes = linkedNodes.ToArray();

        // Remove all references to this face
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].GetComponent<Node>().RemoveFace(this.gameObject);
        }

        // Check if the nodes should still be linked to eachother after face is removed
        for (int i = 0; i < nodes.Length; i++)
        {
            for(int j = 0; j < nodes.Length; j++)
            {
                nodes[i].GetComponent<Node>().IsLinked(nodes[j]);
            }
        }

        Destroy(this.gameObject);
    }

    private void CheckStatus()
    {
        bool claimedNode = false;
        foreach(GameObject node in linkedNodes)
        {
            if (node.GetComponent<Node>().IsClaimed())
            {
                claimedNode = true;
                break;
            }
        }

        if(!claimed && claimedNode)
        {
            claimed = true;
            ren.material.color = claimedColor;
        }
        else if(claimed && !claimedNode)
        {
            claimed = false;
            ren.material.color = Color.white;
        }

        if (claimed && ren.material.color != claimedColor)
            ren.material.color = claimedColor;
    }

    IEnumerator UpdateColor()
    {
        WaitForSeconds delay = new WaitForSeconds(1f);
        while(true)
        {
            yield return delay;
            CheckStatus();
            yield return null;
        }
    }
}
