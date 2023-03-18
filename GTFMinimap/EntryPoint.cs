using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFMinimap.Minimap;
using GTFMinimap.Minimap.Details;
using GTFMinimap.Minimap.Layouts;
using GTFO.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Linq;
using UnityEngine;

namespace GTFMinimap;
[BepInPlugin("GTFMinimap.GUID", VersionInfo.RootNamespace, VersionInfo.Version)]
[BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
internal class EntryPoint : BasePlugin
{
    private Harmony _Harmony;

    public override void Load()
    {
        _Harmony = new Harmony($"{VersionInfo.RootNamespace}.Harmony");
        _Harmony.PatchAll();

        ClassInjector.RegisterTypeInIl2Cpp<MinimapGUI>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapLayouts>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapNavMeshPart>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapDetails>();

        Logger.Info($"Plugin has loaded with {_Harmony.GetPatchedMethods().Count()} patches!");

        AssetAPI.OnAssetBundlesLoaded += AssetAPI_OnAssetBundlesLoaded;
        AssetAPI.OnStartupAssetsLoaded += AssetAPI_OnStartupAssetsLoaded;
    }

    //This will be called first
    private void AssetAPI_OnAssetBundlesLoaded()
    {
        if (Materials.TrySetup())
        {
            
        }
    }

    private void AssetAPI_OnStartupAssetsLoaded()
    {
        Layers.Setup();

        var gameObject = new GameObject(name: "Minimap Handler Object");
        Object.DontDestroyOnLoad(gameObject);

        gameObject.AddComponent<MinimapGUI>();
        gameObject.AddComponent<MinimapLayouts>();
    }
}