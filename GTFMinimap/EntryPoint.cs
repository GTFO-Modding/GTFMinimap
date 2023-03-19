using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFMinimap.Minimap;
using GTFMinimap.Minimap.Details;
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
    private Harmony _Harmony = null;
    private bool _BundleLoaded = false;

    public override void Load()
    {
        _Harmony = new Harmony($"{VersionInfo.RootNamespace}.Harmony");
        _Harmony.PatchAll();

        ClassInjector.RegisterTypeInIl2Cpp<MinimapCam>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapLayout>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapGUI>();
        ClassInjector.RegisterTypeInIl2Cpp<MinimapDetails>();

        Logger.Info($"Plugin has loaded with {_Harmony.GetPatchedMethods().Count()} patches!");

        AssetAPI.OnAssetBundlesLoaded += AssetAPI_OnAssetBundlesLoaded;
        AssetAPI.OnStartupAssetsLoaded += AssetAPI_OnStartupAssetsLoaded;
    }

    public override bool Unload()
    {
        Logger.Error("Unloading Plugin...");
        _Harmony?.UnpatchSelf();
        return base.Unload();
    }

    //This will be called first
    private void AssetAPI_OnAssetBundlesLoaded()
    {
        if (Materials.TrySetup())
        {
            Layers.Setup();
            _BundleLoaded = true;

            Logger.Info("Required Bundle Loaded!");
        }
    }

    private void AssetAPI_OnStartupAssetsLoaded()
    {
        if (_BundleLoaded == false)
        {
            Logger.Error("Required Bundle was NOT loaded!!!");
            Unload();
            return;
        }

        var guiObject = CreatePersistObject("Minimap GUI");
        guiObject.AddComponent<MinimapGUI>();

        var cameraObject = CreatePersistObject("Minimap Camera");
        cameraObject.AddComponent<MinimapCam>();

        var layoutObject = CreatePersistObject("Minimap Layout");
        layoutObject.AddComponent<MinimapLayout>();
    }

    private static GameObject CreatePersistObject(string name)
    {
        var go = new GameObject(name);
        Object.DontDestroyOnLoad(go);
        return go;
    }
}