using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Angles
{
    public GameObject node;
    public float angle;
}


public class Face : IDeletable
{
    [Tooltip("List of nodes associated with this face. Do not edit in editor.")]
    [SerializeField] private List<GameObject> linkedNodes = new List<GameObject>();

    [Tooltip("The transparent material for this face")]
    [SerializeField] private Material transparentMat;
    [Tooltip("The normal material for the border")]
    [SerializeField] private Material borderMat;
    [Tooltip("The claimed material for the border")]
    [SerializeField] private Material claimedBorderMat;
    /// <summary>
    /// Whether or not this face is transparent
    /// </summary>
    private bool isTransparent = false;

    /// <summary>
    /// Whether or not this face is claaimed by an attached node
    /// </summary>
    private bool claimed = false;
    /// <summary>
    /// Public getter for claimed var
    /// </summary>
    public bool Claimed
    {
        get
        {
            return claimed;
        }
    }

    /// <summary>
    /// Mesh renderer for this object
    /// </summary>
    private MeshRenderer ren;
    /// <summary>
    /// renderer for the borders of the face
    /// </summary>
    private LineRenderer lineRen;
    /// <summary>
    /// The center of this face
    /// </summary>
    private Vector3 center;

    /// <summary>
    /// Get the renderer, check its internal color
    /// </summary>
    private void Awake()
    {
        ren = GetComponent<MeshRenderer>();
        lineRen = GetComponent<LineRenderer>();
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
            SortNodes();
        }
        else
        {
            Debug.Log("Not enough nodes to be a face. Deleting self");
            Destroy(this.gameObject);
        }
    }

    #region Setup

    /// <summary>
    /// Sort the nodes in clockwise order
    /// </summary>
    private void SortNodes()
    {
        int plane = GetPlane();

        List<Angles> angles = new List<Angles>();
        Angles temp;

        switch (plane)
        {
            case 0:
                // YZ plane
                foreach (GameObject node in linkedNodes)
                {
                    temp.node = node;
                    temp.angle = QuadrantAngle(node.transform.position.y - center.y, node.transform.position.z - center.z);

                    angles.Add(temp);
                }
                break;
            case 1:
                // XZ plane
                foreach (GameObject node in linkedNodes)
                {
                    temp.node = node;
                    temp.angle = QuadrantAngle(node.transform.position.z - center.z, node.transform.position.x - center.x);

                    angles.Add(temp);
                }
                break;
            case 2:
                // XY plane
                foreach (GameObject node in linkedNodes)
                {
                    temp.node = node;
                    temp.angle = QuadrantAngle(node.transform.position.y - center.y, node.transform.position.x - center.x);

                    angles.Add(temp);
                }
                break;
        }

        List<GameObject> result = SortByAngles(angles);

        linkedNodes = result;

        GenerateFace();
    }


    /// <summary>
    /// Links all nodes with face and eachother data wise
    /// </summary>
    private void GenerateFace()
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
    private void CreateMesh()
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

    /// <summary>
    /// Determine which 2D plane to use for sorting. Finds the axis with smallest variation
    /// </summary>
    /// <returns>The 2D plane that does not contain the axis of least-variation</returns>
    private int GetPlane()
    {
        // Using vector2 to keep track of min/max values. x's are mins, y's are maxes
        Vector2 xVals = new Vector2(linkedNodes[0].transform.position.x, linkedNodes[0].transform.position.x);
        Vector2 yVals = new Vector2(linkedNodes[0].transform.position.y, linkedNodes[0].transform.position.y);
        Vector2 zVals = new Vector2(linkedNodes[0].transform.position.z, linkedNodes[0].transform.position.z);

        // Get the min and max values of each coordinate set
        foreach (GameObject node in linkedNodes)
        {
            if (node.transform.position.x < xVals.x)
            {
                xVals.x = node.transform.position.x;
            }
            else if (node.transform.position.x > xVals.y)
            {
                xVals.y = node.transform.position.x;
            }

            if (node.transform.position.y < yVals.x)
            {
                yVals.x = node.transform.position.y;
            }
            else if (node.transform.position.y > yVals.y)
            {
                yVals.y = node.transform.position.y;
            }

            if (node.transform.position.z < zVals.x)
            {
                zVals.x = node.transform.position.z;
            }
            else if (node.transform.position.z > zVals.y)
            {
                zVals.y = node.transform.position.z;
            }
        }

        float[] differences = new float[] { xVals.y - xVals.x, yVals.y - yVals.x, zVals.y - zVals.x };
        center = new Vector3(xVals.x + differences[0] / 2, yVals.x + differences[1] / 2, zVals.x + differences[2] / 2);

        int minIndex = 0;
        for (int i = 1; i < differences.Length; i++)
        {
            if (differences[i] < differences[minIndex])
            {
                minIndex = i;
            }
        }
        return minIndex;
    }

    /// <summary>
    /// Get the angle in a 360 degree range, compensating for quadrants
    /// </summary>
    /// <param name="y"> the 'y' value being put into arctan</param>
    /// <param name="x"> the 'x' value being put into arctan</param>
    /// <returns>The degree value from the center, [0,360) range</returns>
    private float QuadrantAngle(float y, float x)
    {
        float angle = Mathf.Atan(y / x) * Mathf.Rad2Deg;

        // Quadrant compensation
        if (x < 0 && y > 0)
            angle += 180;
        else if (x < 0 && y < 0)
            angle += 180;
        else if (x > 0 && y < 0)
            angle += 360;

        return angle;
    }

    /// <summary>
    /// Sorts a given list by angle, returns only node objects
    /// </summary>
    /// <param name="data">List of angles to sort through</param>
    /// <returns>The sorted array of gameobjects</returns>
    private List<GameObject> SortByAngles(List<Angles> data)
    {
        List<GameObject> result = new List<GameObject>();
        int count = data.Count;

        for (int i = 0; i < count; i++)
        {
            int min = 0;

            for (int j = 0; j < data.Count; j++)
            {
                if (data[j].angle < data[min].angle)
                {
                    min = j;
                }
            }
            result.Add(data[min].node);
            //print(data[min].node.GetComponent<Node>().id + " " + data[min].angle);
            data.RemoveAt(min);
        }

        return result;
    }

    #endregion

    #region Color Management

    /// <summary>
    /// Checks if this face is claimed
    /// </summary>
    private void CheckStatus()
    {
        // Update claimed status
        bool claimedNode = false;

        foreach (GameObject node in linkedNodes)
        {
            if (node.GetComponent<Node>().IsClaimed())
            {
                claimedNode = true;
                break;
            }
        }

        claimed = claimedNode;

        // If color locked, don't change colors
        if (colorLocked)
        {
            return;
        }

        // Update the borders. Done even if marked as transparent
        if (!claimed && lineRen.material != borderMat)
        {
            lineRen.material = borderMat;
        }
        else if (claimed && lineRen.material != claimedBorderMat)
        {
            lineRen.material = claimedBorderMat;
        }

        // Update the color appropriately
        if (isTransparent && ren.material != transparentMat)
        {
            ren.material = transparentMat;
        }
        else if (!claimed && ren.material != normalMaterial)
        {
            ren.material = normalMaterial;
        }
        else if (claimed && ren.material != claimedMaterial)
        {
            ren.material = claimedMaterial;
        }
    }

    /// <summary>
    /// Occasionally update the color. Do periodically to reduce calls
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateColor()
    {
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        while (true)
        {
            yield return delay;
            CheckStatus();
            yield return null;
        }
    }

    /// <summary>
    /// Toggle the transpacency of the face
    /// </summary>
    public void ToggleTransparency()
    {
        isTransparent = !isTransparent;
    }

    #endregion


    /// <summary>
    /// Get all nodes this face is attached to
    /// </summary>
    /// <returns>List of node objects</returns>
    public List<GameObject> GetNodes()
    {
        return linkedNodes;
    }

    /// <summary>
    /// Deletes the face object and references
    /// </summary>
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
}
