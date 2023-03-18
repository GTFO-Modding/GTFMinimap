using AIGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTFMinimap.Minimap.Layouts;
internal sealed class MinimapNavMeshPart : MonoBehaviour
{
    public int CourseNodeID => CourseNode.NodeID;
    public AIG_CourseNode CourseNode { get; set; }

    private readonly List<GameObject> _MeshObjects = new();

    void OnDestroy()
    {
        Clear();
    }

    public void AddMesh(Vector3[] vertics, int[] triangles)
    {
        var gameObject = new GameObject("MeshLayoutPart");
        gameObject.transform.parent = transform;
        gameObject.transform.position = Vector3.zero;
        gameObject.layer = Layers.Layer_Minimap;

        var filter = gameObject.AddComponent<MeshFilter>();
        var renderer = gameObject.AddComponent<MeshRenderer>();

        var mesh = new Mesh();
        mesh.indexFormat = Mathf.Max(vertics.Length, triangles.Length) >= ushort.MaxValue - 1
            ? IndexFormat.UInt32
            : IndexFormat.UInt16;
        mesh.vertices = vertics;
        mesh.triangles = triangles;

        filter.mesh = mesh;

        _MeshObjects.Add(gameObject);
    }

    public void ClearMesh()
    {
        foreach (var obj in _MeshObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
    }

    public void Clear()
    {
        ClearMesh();
        Destroy(this);
    }
}
