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
    private bool _IsVisible = false;

    void Update()
    {
        var localPlayer = PlayerManager.GetLocalPlayerAgent();
        if (localPlayer == null)
            return;
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

        if (MinimapCam.RenderTexture == null)
            return;

        GUI.Box(new Rect(100.0f, 100.0f, 250.0f, 250.0f), GUIContent.none);
        GUI.DrawTexture(new Rect(100.0f, 100.0f, 250.0f, 250.0f), MinimapCam.RenderTexture, ScaleMode.ScaleAndCrop);
        GUI.Box(new Rect(225.0f, 225.0f, 2.0f, 2.0f), GUIContent.none);
    }
}
