using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutoBuilder : MonoBehaviour
{
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] SelectionSystem selectionSystem;
    Dictionary<uint, Node> nodes;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void BuildFromFile(string points="points.txt", string faces="faces.txt")
    {
        nodes = new Dictionary<uint, Node>();
        // Reading vertices
        StreamReader f_points = new StreamReader(points);
        string nextCoord_str = "init";
        while (nextCoord_str != null)
        {
            nextCoord_str = f_points.ReadLine();
            uint nextId;
            Vector4 nextCoord = CsvToVec4AndId(nextCoord_str, out nextId);
            spawnManager.transform.position = new Vector3(nextCoord[0], nextCoord[1], nextCoord[2]); // Throws out the fourth dimension
            Node node = spawnManager.SpawnNode();
            node.id = nextId;
            nodes[nextId] = node;
        }

        // Reading faces
        StreamReader f_faces = new StreamReader(faces);
        string nextFace_str = "init";
        while (nextFace_str != null)
        {
            nextFace_str = f_faces.ReadLine();
            List<uint> nextVerts = CsvToUints(nextFace_str);
            List<GameObject> nextNodes = new List<GameObject>();
            foreach(uint id in nextVerts)
            {
                nextNodes.Add(nodes[id].gameObject);
            }
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
}
