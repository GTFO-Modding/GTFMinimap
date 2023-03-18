using BepInEx;
using Il2CppInterop.Runtime.Attributes;
using Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFMinimap.Minimap;

internal sealed partial class MinimapGUI : MonoBehaviour
{
    private const float CAM_DEFAULT_CEILING = 25.0f;
    private const float CAM_MAX_DISTANCE = 50.0f;
    private const float CAM_FOV = 110.0f;

    private Camera _Cam;
    private RenderTexture _RenderTexture;

    [HideFromIl2Cpp]
    void Start_AddCamera()
    {
        _RenderTexture = new RenderTexture(200, 200, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        {
            name = "minimap_RenderTexture",
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        _RenderTexture.Create();
        _RenderTexture.hideFlags = HideFlags.HideAndDontSave;

        _Cam = gameObject.AddComponent<Camera>();
        _Cam.orthographic = true;
        _Cam.orthographicSize = 15.0f;
        _Cam.fieldOfView = CAM_FOV;
        _Cam.nearClipPlane = 0f;
        _Cam.farClipPlane = CAM_MAX_DISTANCE;
        _Cam.cullingMask = _Mask_MinimapItems;
        _Cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        _Cam.allowMSAA = false;
        _Cam.allowHDR = false;
        _Cam.useOcclusionCulling = false;
        _Cam.renderingPath = RenderingPath.Forward;
        _Cam.clearFlags = CameraClearFlags.SolidColor;
        _Cam.backgroundColor = Color.blue.RGBMultiplied(0.15f);
        _Cam.targetTexture = _RenderTexture;
    }

    [HideFromIl2Cpp]
    void Update_CameraTransform(PlayerAgent localPlayer)
    {
        var camTransform = _Cam.gameObject.transform;

        var pos = localPlayer.Position;
        Materials.UpdatePlayerPosition(pos);

        var ceilingHeight = CAM_DEFAULT_CEILING;
        /*
        var hasCeiling = Physics.Raycast(pos, Vector3.up, out RaycastHit hitInfo, 20.0f, LayerManager.MASK_WORLD);
        if (hasCeiling)
        {
            ceilingHeight = hitInfo.distance;
        }
        */

        pos += Vector3.up * ceilingHeight;

        camTransform.position = pos;
        camTransform.rotation = Quaternion.LookRotation(Vector3.down, localPlayer.transform.forward);
    }

    [HideFromIl2Cpp]
    void RenderCameraToFile()
    {
        RenderTexture rt = _RenderTexture;
        RenderTexture oldRT = RenderTexture.active;

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = oldRT;

        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Paths.PluginPath, "mapImage.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved to " + path);
    }
}
