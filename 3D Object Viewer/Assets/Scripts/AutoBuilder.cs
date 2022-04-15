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
        BuildFromFile("hypercube coordinates", "hypercube faces");
    }

    void Update()
    {
        
    }

    public void BuildFromFile(string points= "hypercube coordinates", string faces="hypercube faces", float factor=10)
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
            Vector4 nextCoord = CsvToVec4AndId(line, out nextId);
            print($"Creating vert at: {nextCoord}");

            // Do stuff with coordinate
            // Create modified 

            // Spawn node
            float fourthDimScale = 1 + nextCoord[3];
            spawnTarget.transform.position = factor * fourthDimScale * new Vector3(nextCoord[0], nextCoord[1], nextCoord[2]); // Throws out the fourth dimension
            Node node = spawnManager.SpawnNode();
            node.id = nextId;
            nodes[nextId] = node;
        }

        // Reading faces
        foreach (string line in lines_faces)
        {
            List<uint> nextVerts = CsvToUints(line);

            List<GameObject> nextNodes = new List<GameObject>();
            foreach(uint id in nextVerts)
            {
                nextNodes.Add(nodes[id].gameObject);
            }

            // Do stuff with vert list
            // Spawn face
            print($"Creating face from verts: {ListToString(nextNodes)}");
            selectionSystem.SpawnFace(nextNodes);
        }
    }

    Vector4 CsvToVec4AndId(string csvLine, out uint vertId)
    {
        Vector4 vec4 = new Vector4();
        string[] strs = csvLine.Split(',');
        try
        {
            vertId = uint.Parse(strs[0]);
            vec4[0] = int.Parse(strs[1]);
            vec4[1] = int.Parse(strs[2]);
            vec4[2] = int.Parse(strs[3]);
            vec4[3] = int.Parse(strs[4]);
        }
        catch
        {
            print($"Failure in line: {csvLine}");
            vertId = 999;
            return vec4;
        }
        return vec4;
    }

    List<uint> CsvToUints(string csvLine)
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
            return uints;
        }

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
