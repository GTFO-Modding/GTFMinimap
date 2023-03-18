using UnityEngine;

namespace GTFMinimap.Minimap;
internal static class Layers
{
    public static int Layer_Minimap { get; private set; }
    public static int Mask_Minimap { get; private set; }

    public static void Setup()
    {
        Layer_Minimap = LayerMask.NameToLayer("InvisibleWall");
        Mask_Minimap = LayerMask.GetMask("InvisibleWall");
    }
}
