using AIGraph;
using GTFO.API;
using Il2CppInterop.Runtime.Attributes;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace GTFMinimap.Minimap.Layouts;
internal sealed class MinimapLayouts : MonoBehaviour
{
    private readonly List<MinimapNavMeshPart> _LayoutParts = new();

    void Start()
    {
        LevelAPI.OnBuildDone += OnBuildDone;
        LevelAPI.OnLevelCleanup += OnLevelCleanup;
    }

    [HideFromIl2Cpp]
    private void OnBuildDone()
    {

    }

    [HideFromIl2Cpp]
    private void OnLevelCleanup()
    {
        foreach (var part in _LayoutParts)
        {
            if (part != null)
                part.Clear();
        }

        _LayoutParts.Clear();
    }

    [HideFromIl2Cpp]
    private bool IsValidNavMesh(Vector3 centerPosition, out AIG_CourseNode ownerNode)
    {
        if (!TryGetCourseNode(Vector3.zero, out var courseNode))
        {
            ownerNode = null;
            return false;
        }

        if (!courseNode.TryGetValidSpawnNode(out var iNode))
        {
            ownerNode = null;
            return false;
        }

        var from = Vector3.zero;
        var to = iNode.Position;
        var hasConnect = NavMesh.CalculatePath(from, to, -1, new NavMeshPath());

        ownerNode = courseNode;
        return hasConnect;
    }

    [HideFromIl2Cpp]
    private bool TryGetCourseNode(Vector3 centerPosition, out AIG_CourseNode courseNode)
    {
        var hasNearGround = Physics.SphereCast(centerPosition, 2.5f, Vector3.up, out var hitInfo, 0.0f, LayerManager.MASK_UNITY_NAVMESH_GENERATION);
        if (!hasNearGround)
        {
            courseNode = null;
            return false;
        }

        var newPos = hitInfo.point;

        var dimension = Dimension.GetDimensionFromPos(newPos);
        if (dimension == null)
        {
            courseNode = null;
            return false;
        }

        if (AIG_GeomorphNodeVolume.TryGetCourseNode(dimension.DimensionIndex, newPos, out courseNode))
        {
            courseNode = null;
            return true;
        }
        else
        {
            courseNode = null;
            return false;
        }
    }
}
