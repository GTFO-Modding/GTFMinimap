using AIGraph;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace GTFMinimap.Minimap.Utils;
internal static class MeshUtil
{
    public static void RemoveInvalidNavMeshTriangles(Mesh baseMesh)
    {
        var vertices = (Vector3[])baseMesh.vertices;
        var triangles = (int[])baseMesh.triangles;
        var trianglesCount = triangles.Length / 3;

        var newTriangles = new List<int>();

        for (int i = 0; i<trianglesCount; i++)
        {
            var p0 = triangles[i * 3];
            var p1 = triangles[(i * 3) + 1];
            var p2 = triangles[(i * 3) + 2];

            CalcTriangleProps(vertices[p0], vertices[p1], vertices[p2],
                out var center,
                out var heightDifference);
            var dimension = Dimension.GetDimensionFromPos(center);
            if (dimension == null)
            {
                continue;
            }

            var tolerance = Mathf.Max(1.5f, heightDifference * 1.5f);
            if (!IsPositionValid(dimension.DimensionIndex, center, tolerance))
            {
                continue;
            }

            newTriangles.Add(p0);
            newTriangles.Add(p1);
            newTriangles.Add(p2);
        }

        baseMesh.triangles = newTriangles.ToArray();
        baseMesh.RecalculateNormals();
        newTriangles.Clear();
    }

    private static bool IsPositionValid(eDimensionIndex dimensionIndex, Vector3 position, float voxelSearchTolerance)
    {
        if (!AIG_GeomorphNodeVolume.TryGetGeomorphVolume(0, dimensionIndex, position, out var gnv))
        {
            //DisplayMarker("GNV Missing!", position);
            return false;
        }

        if (!gnv.m_voxelNodeVolume.TryGetPillar(position, out var vnp))
        {
            //DisplayMarker("Pillar Missing!", position);
            return false;
        }

        var height = position.y;
        if (!vnp.TryGetVoxelNode(height - voxelSearchTolerance, height + voxelSearchTolerance, out var iNode))
        {
            //DisplayMarker("iNode Missing!", position);
            return false;
        }

        if (!AIG_NodeCluster.TryGetNodeCluster(iNode.ClusterID, out var nodeCluster))
        {
            return false;
        }

        return true;
    }

    private static void CalcTriangleProps(Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 center, out float heightDifference)
    {
        center = (p1 + p2 + p3) * 0.333333f;

        var lowest = Mathf.Min(p1.y, p2.y, p3.y);
        var highest = Mathf.Max(p1.y, p2.y, p3.y);

        heightDifference = highest - lowest;
    }

    /*
    public static void RemoveInaccessibleMesh(Mesh baseMesh)
    {
        Stopwatch w = new();

        w.Restart();
        var vertices = (Vector3[])baseMesh.vertices;
        var dimensions = new eDimensionIndex[vertices.Length];
        var isVerticesInvalid = new bool[vertices.Length];


        w.Stop();
        Logger.Info($"Batch/ Conv Vertices, Init: {w.ElapsedMilliseconds}ms");

        w.Restart();
        for (int i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];
            var d = Dimension.GetDimensionFromPos(v);

            if (d == null)
            {
                isVerticesInvalid[i] = true;
                Logger.Error("Found Faulty Position!!!!!");
                continue;
            }

            dimensions[i] = d.DimensionIndex;
            isVerticesInvalid[i] = false;
        }


        w.Stop();
        Logger.Info($"Batch/ Look for Dimensions: {w.ElapsedMilliseconds}ms");

        w.Restart();
        for (int i = 0; i < vertices.Length; i++){
            if (isVerticesInvalid[i])
                continue;

            var dimensionIndex = dimensions[i];
            var position = vertices[i];
            isVerticesInvalid[i] = !IsPositionValid(dimensionIndex, position);
        }

        w.Stop();
        Logger.Info($"Batch/ Check if Valid: {w.ElapsedMilliseconds}ms");

        var total = isVerticesInvalid.Length;
        var count = 0;
        foreach (var isInvalid in isVerticesInvalid)
        {
            if (isInvalid) count++;
        }

        Logger.Info("Mesh Reformatted!!!");
        Logger.Info($"Invalid Vertices Count: {count}/{total} ({count / (float)total})");
        RemoveInvalidTriangles(baseMesh, isVerticesInvalid);
    }
    */

    

    private static void DisplayMarker(string name, Vector3 position)
    {
        var newobj = new GameObject();
        newobj.transform.position = position;
        GuiManager.NavMarkerLayer.PlaceCustomMarker(NavMarkerOption.Title, newobj, name);
    }

    //TODO: Split-up Meshes by Conntivitiy
    /*
    public static void SplitMeshByConnectivity(Mesh baseMesh, out Mesh[] subMeshes)
    {
        var tempSubMeshes = new List<Mesh>();
        var tempTriangleGroups = new List<TriangleGroup>();

        subMeshes = tempSubMeshes.ToArray();

        var triangles = (int[])baseMesh.triangles;
        var triangleCount = triangles.Length / 3;

        for (int i = 0; i<triangleCount; i++)
        {
            
        }

        foreach (var group in tempTriangleGroups)
        {

        }
        
        tempSubMeshes.Clear();
    }

    public class TriangleGroup
    {
        private readonly List<int> _Triangles = new();
        private readonly List<int> _VerticesList = new();

        public Mesh CreateMesh(Vector3[] vertices)
        {
            var tempVertices = new List<int>();
            var triangleCount = _Triangles.Count / 3;
            for (int i = 0; i < triangleCount; i++)
            {

            }
            return null;
        }

        public bool TryPutTriangle(int p1_index, int p2_index, int p3_index)
        {
            if (ContainsVerticesIndex(p1_index, p2_index, p3_index))
            {
                _Triangles.Add(p1_index);
                _Triangles.Add(p2_index);
                _Triangles.Add(p3_index);
                return true;
            }

            return false;
        }

        public bool ContainsVerticesIndex(int a, int b, int c)
        {
            foreach (var vertices in _VerticesList)
            {
                if (a == vertices)
                    return true;

                if (b == vertices)
                    return true;

                if (c == vertices)
                    return true;
            }

            return false;
        }
    }
    */
}
