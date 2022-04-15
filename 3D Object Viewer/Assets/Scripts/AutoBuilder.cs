using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutoBuilder : MonoBehaviour
{
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] GameObject spawnTarget;

    Dictionary<uint, Node> nodes;

    void Start()
    {
        BuildFromFile("600CellVertexCoordinates", "600CellFacesMadeWithCoordinates", 10, 0, 1);
    }

    void Update()
    {
        
    }

    public void BuildFromFile(string points= "hypercube coordinates", string faces="hypercube faces", float sizeFactor=10, float fourthDimSizeOffset=1,float fourthDimSizeFactor=1)
    {
        nodes = new Dictionary<uint, Node>();

        // Load files
        TextAsset txt_points = Resources.Load<TextAsset>(points);
        TextAsset txt_faces = Resources.Load<TextAsset>(faces);
        // Check that they were found
        if(txt_points == null)
        {
            print($"Autobuilder: Vertices file {points} not found");
            return;
        }
        if (txt_faces == null)
        {
            print($"Autobuilder: Faces file {faces} not found");
            return;
        }

        // Split the files into lines
        string[] lines_points = txt_points.text.Split('\n');
        string[] lines_faces = txt_faces.text.Split('\n');
        // Check not empty
        if (lines_points.Length <= 1)
        {
            print($"Autobuilder: Vertices file {points} empty");
            return;
        }
        if (lines_faces.Length <= 1)
        {
            print($"Autobuilder: Faces file {faces} empty");
            return;
        }

        // Reading vertices
        foreach (string line in lines_points)
        {
            uint nextId;
            bool readSuccess;
            Vector4 nextCoord = CsvToVec4AndId(line, out nextId, out readSuccess);
            if (!readSuccess)
            {
                continue;
            }
            print($"Autobuilder: Creating vert at: {nextCoord}");

            // Do stuff with coordinate
            // Modify coordinate for 4th dimension
            float fourthDimScale = fourthDimSizeOffset + fourthDimSizeFactor * nextCoord[3];

            // Spawn node
            spawnTarget.transform.position = sizeFactor * fourthDimScale * new Vector3(nextCoord[0], nextCoord[1], nextCoord[2]); // Throws out the fourth dimension
            Node node = spawnManager.SpawnNode();
            node.id = nextId;
            nodes[nextId] = node;
        }

        // Reading faces
        foreach (string line in lines_faces)
        {
            bool readSuccess;
            List<uint> nextVerts = CsvToUints(line, out readSuccess);
            if (!readSuccess)
            {
                continue;
            }

            List<GameObject> nextNodes = new List<GameObject>();
            foreach(uint id in nextVerts)
            {
                nextNodes.Add(nodes[id].gameObject);
            }

            // Do stuff with vert list
            // Spawn face
            print($"Autobuilder: Creating face from verts: {ListToString(nextNodes)}");
            selectionSystem.SpawnFace(nextNodes);
        }
    }

    Vector4 CsvToVec4AndId(string csvLine, out uint vertId, out bool success)
    {
        Vector4 vec4 = new Vector4();
        string[] strs = csvLine.Split(',');

        try
        {
            vertId = uint.Parse(strs[0]);
        }
        catch
        {
            print($"Failure reading ID in line: {csvLine}");
            vertId = 0;
            success = false;
        }
        try
        {
            vec4[0] = float.Parse(strs[1]);
            vec4[1] = float.Parse(strs[2]);
            vec4[3] = float.Parse(strs[4]);
            vec4[2] = float.Parse(strs[3]);
        }
        catch
        {
            print($"Failure reading coordinates in line: {csvLine}");
            success = false;
        }
        success = true;
        return vec4;
    }

    List<uint> CsvToUints(string csvLine, out bool success)
    {
        List<uint> uints = new List<uint>();
        string[] strs = csvLine.Split(',');
        try
        {
            foreach(string str in strs)
            {
                uints.Add(uint.Parse(str));
            }
        }
        catch
        {
            print($"Failure in line: {csvLine}");
            success = false;
        }
        success = true;
        return uints;
    }

    string ListToString(List<GameObject> list)
    {
        string str = "[";
        foreach(GameObject elem in list)
        {
            str += elem.name + ", ";
        }
        // Chop off last comma and add ]
        return str.Substring(0, str.Length-1) + "]";
    }
}
