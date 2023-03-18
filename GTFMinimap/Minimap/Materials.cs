using GTFO.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFMinimap.Minimap;

internal struct LayoutMatParams
{
    private static readonly int _Prop_Color = Shader.PropertyToID("_Color");
    private static readonly int _Prop_HeightRange = Shader.PropertyToID("_HeightRange");
    private static readonly int _Prop_FadeStart = Shader.PropertyToID("_FadeStart");
    private static readonly int _Prop_FadeEnd = Shader.PropertyToID("_FadeEnd");

    public Color TintColor; //_Color
    public float HeightRange; //_HeightRange
    public float FadeStart; //_FadeStart
    public float FadeEnd; //_FadeEnd

    public void ApplyTo(Material material)
    {
        material.SetColor(_Prop_Color, TintColor);
        material.SetFloat(_Prop_HeightRange, HeightRange);
        material.SetFloat(_Prop_FadeStart, FadeStart);
        material.SetFloat(_Prop_FadeEnd, FadeEnd);
    }
}

internal struct MarkerMatParams
{
    private static readonly int _Prop_Color = Shader.PropertyToID("_Color");
    private static readonly int _Prop_FadeStart = Shader.PropertyToID("_FadeStart");
    private static readonly int _Prop_FadeEnd = Shader.PropertyToID("_FadeEnd");

    public Color TintColor; //_Color
    public float FadeStart; //_FadeStart
    public float FadeEnd; //_FadeEnd

    public void ApplyTo(Material material)
    {
        material.SetColor(_Prop_Color, TintColor);
        material.SetFloat(_Prop_FadeStart, FadeStart);
        material.SetFloat(_Prop_FadeEnd, FadeEnd);
    }
}

internal static class Materials
{
    //Base Materials
    public static Material LayoutBaseMat { get; private set; }
    public static Material MarkerBaseMat { get; private set; }

    //
    private static int _Prop_PlayerPos;

    public static bool TrySetup()
    {
        LayoutBaseMat = AssetAPI.GetLoadedAsset<Material>("Assets/Minimap/Materials/Layout.mat");
        MarkerBaseMat = AssetAPI.GetLoadedAsset<Material>("Assets/Minimap/Materials/Marker.mat");

        _Prop_PlayerPos = Shader.PropertyToID("_Minimap__PlayerPosition");

        var hasAsset = LayoutBaseMat != null && MarkerBaseMat != null;
        if (hasAsset)
        {
            CreateCopies();
        }

        return hasAsset;
    }

    private static void CreateCopies()
    {
        CreateMarkerMaterial("Locker", new()
        {
            TintColor = ColorExt.Hex("#ff9e00"),
            FadeStart = 6.0f,
            FadeEnd = 10.0f
        });

        CreateMarkerMaterial("Box", new()
        {
            TintColor = ColorExt.Hex("#ff9e00"),
            FadeStart = 6.0f,
            FadeEnd = 10.0f
        });
    }

    private static Material CreateLayoutMaterial(string name, LayoutMatParams param)
    {
        var newMat = new Material(LayoutBaseMat)
        {
            name = name,
            hideFlags = HideFlags.HideAndDontSave
        };
        param.ApplyTo(newMat);
        return newMat;
    }

    private static Material CreateMarkerMaterial(string name, MarkerMatParams param)
    {
        var newMat = new Material(MarkerBaseMat)
        {
            name = name,
            hideFlags = HideFlags.HideAndDontSave
        };
        param.ApplyTo(newMat);
        return newMat;
    }

    public static void UpdatePlayerPosition(Vector3 position)
    {
        Shader.SetGlobalVector(_Prop_PlayerPos, position);
    }
}
