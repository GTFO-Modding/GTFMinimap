using BepInEx.Unity.IL2CPP.Utils;
using GTFMinimap.Minimap.Utils;
using GTFO.API;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace GTFMinimap.Minimap;
internal class MinimapLayout : MonoBehaviour
{
    public static int CeilingColliderID { get; private set; }

    private Mesh _NavMesh;

    private MaterialPropertyBlock _PropertyBlock = new();
    private Coroutine _TunnelVisionRoutine = null;
    private int _TVRadiusID = Shader.PropertyToID("_TV_Radius");

    private static MinimapLayout _Instance;

    private MeshRenderer _Renderer;
    private MeshFilter _Filter;

    private GameObject _ColliderObject;
    private MeshFilter _ColliderFilter;

    public static void EnableTunnelVision()
    {
        if (_Instance._TunnelVisionRoutine != null)
        {
            _Instance.StopCoroutine(_Instance._TunnelVisionRoutine);
        }

        _Instance.StartCoroutine(_Instance.Routine_ChangeTunnelVisionRadius(6.0f));
    }

    public static void DisableTunnelVision()
    {
        if (_Instance._TunnelVisionRoutine != null)
        {
            _Instance.StopCoroutine(_Instance._TunnelVisionRoutine);
        }

        _Instance.StartCoroutine(_Instance.Routine_ChangeTunnelVisionRadius(0.0f));
    }

    void Start()
    {
        _Instance = this;

        _Renderer = gameObject.AddComponent<MeshRenderer>();
        _Renderer.material = Materials.LayoutBaseMat;
        _Filter = gameObject.AddComponent<MeshFilter>();

        _ColliderObject = new GameObject(name: "TunnelVision Collider");
        _ColliderObject.AddComponent<MeshCollider>();
        _ColliderObject.layer = Layers.Layer_MinimapCeilingCheck;
        _ColliderObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        CeilingColliderID = _ColliderObject.GetInstanceID();

        _ColliderFilter = _ColliderObject.AddComponent<MeshFilter>();
        DontDestroyOnLoad(_ColliderObject);
        

        gameObject.layer = Layers.Layer_Minimap;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        LevelAPI.OnBuildDone += OnBuildDone;
        LevelAPI.OnLevelCleanup += OnLevelCleanup;
    }

    [HideFromIl2Cpp]
    private void OnBuildDone()
    {
        ClearMesh();
        CreateNavMesh();
    }

    [HideFromIl2Cpp]
    private void OnLevelCleanup()
    {
        ClearMesh();
    }

    [HideFromIl2Cpp]
    void ClearMesh()
    {
        if (_NavMesh != null)
        {
            Destroy(_NavMesh);
            _NavMesh = null;
        }

        _Filter.mesh = null;
        _ColliderFilter.mesh = null;
    }

    [HideFromIl2Cpp]
    void CreateNavMesh()
    {
        //Build Mesh using NavMesh Info
        _NavMesh = new Mesh();

        var trigs = NavMesh.CalculateTriangulation();
        CalcMesh(trigs, out var format, out var vertices, out var triangles);
        _NavMesh.indexFormat = format;
        _NavMesh.vertices = vertices;
        _NavMesh.triangles = triangles;

        MeshUtil.RemoveInvalidNavMeshTriangles(_NavMesh);

        //Apply Mesh
        _Filter.mesh = _NavMesh;
        _ColliderFilter.mesh = _NavMesh;
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

    [HideFromIl2Cpp]
    IEnumerator Routine_ChangeTunnelVisionRadius(float targetRadius)
    {
        float time = 0.0f;
        float endTime = 0.5f;
        float currentRadius = _PropertyBlock.GetFloat(_TVRadiusID);

        while (time <= endTime)
        {
            time += Time.deltaTime;

            var radius = Mathf.Lerp(currentRadius, targetRadius, time / endTime);
            _PropertyBlock.SetFloat(_TVRadiusID, radius);
            _Renderer.SetPropertyBlock(_PropertyBlock);
            yield return null;
        }

        _PropertyBlock.SetFloat(_TVRadiusID, targetRadius);
        _Renderer.SetPropertyBlock(_PropertyBlock);
    }
}
