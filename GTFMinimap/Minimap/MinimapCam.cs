using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFMinimap.Minimap;
internal class MinimapCam : MonoBehaviour
{
    public const float CAM_DEFAULT_HEIGHT = 25.0f;
    public readonly Vector3 CAM_DEFAULT_HEIGHT_VECTOR = Vector3.up * CAM_DEFAULT_HEIGHT;
    public const float CAM_MAX_DISTANCE = 50.0f;
    public const float CAM_FOV = 110.0f;

    private PlayerAgent _LocalPlayer = null;
    private Il2CppStructArray<RaycastHit> _RayResults = new(100);

    public static Camera Cam { get; private set; }
    public static RenderTexture RenderTexture { get; private set; }
    public static bool IsUnderCeiling { get; private set; }

    void Start()
    {
        RenderTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        {
            name = "MiniMap RenderTexture",
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        RenderTexture.Create();
        RenderTexture.hideFlags = HideFlags.HideAndDontSave;

        Cam = gameObject.AddComponent<Camera>();
        Cam.orthographic = true;
        Cam.orthographicSize = 15.0f;
        Cam.fieldOfView = CAM_FOV;
        Cam.nearClipPlane = 0f;
        Cam.farClipPlane = CAM_MAX_DISTANCE;
        Cam.cullingMask = Layers.Mask_Minimap;
        Cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        Cam.allowMSAA = false;
        Cam.allowHDR = false;
        Cam.useOcclusionCulling = false;
        Cam.renderingPath = RenderingPath.Forward;
        Cam.clearFlags = CameraClearFlags.SolidColor;
        Cam.backgroundColor = Color.white.RGBMultiplied(0.13f);
        Cam.targetTexture = RenderTexture;

        this.StartCoroutine(CeilingCheck());
    }

    [HideFromIl2Cpp]
    IEnumerator CeilingCheck()
    {
        var waiter = new WaitForSecondsRealtime(0.1f);
        while(true)
        {
            if (_LocalPlayer != null)
            {
                var camPos = Cam.transform.position;
                var playerPos = _LocalPlayer.Position;
                var rayLength = Vector3.Distance(camPos, playerPos);
                var count = Physics.RaycastNonAlloc(camPos, Vector3.down, _RayResults, rayLength, Layers.Mask_MinimapCeilingCheck);

                var foundCeiling = false;
                for(int i = 0; i<count; i++)
                {
                    var objectID = _RayResults[i].transform.gameObject.GetInstanceID();
                    if (objectID == MinimapLayout.CeilingColliderID)
                    {
                        foundCeiling = true;
                        break;
                    }
                }

                if (foundCeiling != IsUnderCeiling)
                {
                    Logger.Error($"Found Ceiling Result Changed: {foundCeiling}");

                    if (foundCeiling) MinimapLayout.EnableTunnelVision();
                    else MinimapLayout.DisableTunnelVision();
                    IsUnderCeiling = foundCeiling;
                }
            }

            yield return waiter;
        }
    }

    void FixedUpdate()
    {
        _LocalPlayer = PlayerManager.GetLocalPlayerAgent();
    }

    void Update()
    {
        if (_LocalPlayer != null)
        {
            Materials.UpdatePlayerPosition(_LocalPlayer.Position);

            var pos = _LocalPlayer.Position + CAM_DEFAULT_HEIGHT_VECTOR;
            var rot = Quaternion.LookRotation(Vector3.down, _LocalPlayer.transform.forward);
            Cam.transform.SetPositionAndRotation(pos, rot);
        }
    }
}
