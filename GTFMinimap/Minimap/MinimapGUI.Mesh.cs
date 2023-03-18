using GTFMinimap.Minimap.Utils;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace GTFMinimap.Minimap;
internal sealed partial class MinimapGUI : MonoBehaviour
{
    private Mesh _NavMesh;
    private MeshRenderer _NavMeshRenderer;
    private GameObject _NavMeshObject;

    [HideFromIl2Cpp]
    void ClearMesh()
    {
        if (_NavMeshObject != null)
        {
            Destroy(_NavMeshObject);
            _NavMeshObject = null;
            _NavMeshRenderer = null;
        }

        if (_NavMesh != null)
        {
            Destroy(_NavMesh);
            _NavMesh = null;
        }
    }

    [HideFromIl2Cpp]
    void CreateNavMesh()
    {
        //Create Object
        _NavMeshObject = new GameObject("NavMeshObject");
        _NavMeshObject.transform.position = Vector3.zero;
        _NavMeshObject.layer = Layers.Layer_Minimap;

        //Add Comp
        var filter = _NavMeshObject.AddComponent<MeshFilter>();
        _NavMeshRenderer = _NavMeshObject.AddComponent<MeshRenderer>();
        _NavMeshRenderer.material = Materials.LayoutBaseMat;

        //Build Mesh using NavMesh Info
        _NavMesh = new Mesh();

        var trigs = NavMesh.CalculateTriangulation();
        CalcMesh(trigs, out var format, out var vertices, out var triangles);
        _NavMesh.indexFormat = format;
        _NavMesh.vertices = vertices;
        _NavMesh.triangles = triangles;

        MeshUtil.RemoveInaccessibleMesh(_NavMesh);

        //Apply Mesh
        filter.mesh = _NavMesh;
    }

    [HideFromIl2Cpp]
    static void CalcMesh(NavMeshTriangulation trigs, out IndexFormat indexFormat, out Vector3[] newVertics, out int[] newTriangles)
    {
        Vector3[] vertics = trigs.vertices;
        int[] triangles = trigs.indices.ToArray();

        indexFormat = (Mathf.Max(vertics.Length, triangles.Length) >= 65534) ? IndexFormat.UInt32 : IndexFormat.UInt16;
        newVertics = vertics;
        newTriangles = triangles;
    }


}
