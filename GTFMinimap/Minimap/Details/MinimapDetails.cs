using GTFO.API;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFMinimap.Minimap.Details;
internal sealed class MinimapDetails : MonoBehaviour
{
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
        
    }
}
