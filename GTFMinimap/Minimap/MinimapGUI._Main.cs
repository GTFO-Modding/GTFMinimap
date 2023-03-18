using BepInEx;
using GTFMinimap.Minimap.Utils;
using GTFO.API;
using Il2CppInterop.Runtime.Attributes;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace GTFMinimap.Minimap;
internal sealed partial class MinimapGUI : MonoBehaviour
{
    private int _Layer_MinimapItems;
    private int _Mask_MinimapItems;

    private bool _IsVisible = false;
    

    void Start()
    {
        _Layer_MinimapItems = LayerMask.NameToLayer("InvisibleWall");
        _Mask_MinimapItems = LayerMask.GetMask("InvisibleWall");

        Start_AddCamera();

        LevelAPI.OnBuildDone += OnBuildDone;
        LevelAPI.OnLevelCleanup += OnLevelCleanup;
    }


    [HideFromIl2Cpp]
    private void OnBuildDone()
    {
        var trigs = NavMesh.CalculateTriangulation();
        Logger.Info($"Trig Info: vert count:{trigs.vertices.Length}, indics count: {trigs.indices.Length}");

        File.WriteAllLines(Path.Combine(Paths.PluginPath, "areaDump.txt"), trigs.areas.Select(x=>x.ToString()));

        ClearMesh();
        CreateNavMesh();
    }

    [HideFromIl2Cpp]
    private void OnLevelCleanup()
    {
        ClearMesh();
    }

    void Update()
    {
        var localPlayer = PlayerManager.GetLocalPlayerAgent();
        if (localPlayer == null)
            return;

        Update_CameraTransform(localPlayer);
    }

    void FixedUpdate()
    {
        var visibleFlag1 = GameStateManager.CurrentStateName == eGameStateName.InLevel;

        var state = FocusStateManager.CurrentState;
        var visibleFlag2 = state == eFocusState.FPS 
            || state == eFocusState.FPS_TypingInChat
            || state == eFocusState.FPS_CommunicationDialog;
        _IsVisible = visibleFlag1 && visibleFlag2;
    }

    void OnGUI()
    {
        if (!_IsVisible)
            return;

        if (_RenderTexture == null)
            return;

        GUI.Box(new Rect(100.0f, 100.0f, 250.0f, 250.0f), GUIContent.none);
        GUI.DrawTexture(new Rect(100.0f, 100.0f, 250.0f, 250.0f), _RenderTexture, ScaleMode.ScaleAndCrop);
        GUI.Box(new Rect(225.0f, 225.0f, 2.0f, 2.0f), GUIContent.none);
        
    }
}
