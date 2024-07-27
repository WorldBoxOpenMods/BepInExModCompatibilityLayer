using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BepinexModCompatibilityLayer {
  /// <summary>
  /// This class serves as an entry point for the utilities provided by the compatibility layer.
  /// </summary>
  [BepInPlugin(BepinexModCompatibilityLayerConfig.PluginGuid, BepinexModCompatibilityLayerConfig.PluginName, BepinexModCompatibilityLayerConfig.PluginVersion)]
  public class ModCompatibilityLayer : BaseUnityPlugin {
    /// <summary>
    /// This field contains the only instance of the <see cref="BepinexModCompatibilityLayerWindowIdSeparator"/> class.
    /// </summary>
    public static readonly BepinexModCompatibilityLayerWindowIdSeparator WindowIdSeparator = new BepinexModCompatibilityLayerWindowIdSeparator();

    /// <summary>
    /// This field contains the only instance of the <see cref="BepinexModCompatibilityLayerButtonGenerator"/> class.
    /// </summary>
    public static readonly BepinexModCompatibilityLayerButtonGenerator ButtonGenerator = new BepinexModCompatibilityLayerButtonGenerator();

    internal void Awake() {
      Logger.LogInfo("Started loading BepinexModCompatibilityLayer...");
      if (BepinexModCompatibilityLayerConfig.EnableWindowIdSeparator) {
        Logger.LogInfo("Patching GUILayout.DoWindow...");
        Harmony harmony = new Harmony(BepinexModCompatibilityLayerConfig.PluginGuid);
        MethodInfo original = AccessTools.Method(typeof(GUILayout), typeof(GUILayout).GetMethod("DoWindow", BindingFlags.Static | BindingFlags.NonPublic)?.Name); 
        MethodInfo prefix = AccessTools.Method(typeof(ModCompatibilityLayer), nameof(DoWindow_Prefix));
        harmony.Patch(original, new HarmonyMethod(prefix));
        Logger.LogInfo("Patched GUILayout.DoWindow!");
      }

      if (BepinexModCompatibilityLayerConfig.EnableButtonGenerator) {
        Logger.LogInfo("Patching GUIUtility.GetControlID...");
        Harmony harmony = new Harmony(BepinexModCompatibilityLayerConfig.PluginGuid);
        MethodInfo original = AccessTools.Method(typeof(GUI), typeof(GUI).GetMethod("DoButton", BindingFlags.Static | BindingFlags.NonPublic)?.Name);
        MethodInfo prefix = AccessTools.Method(typeof(ModCompatibilityLayer), nameof(DoButton_Prefix));
        harmony.Patch(original, new HarmonyMethod(prefix));
        Logger.LogInfo("Patched GUIUtility.GetControlID!");
        Logger.LogInfo("Patching GUIClip.Push...");
        harmony = new Harmony(BepinexModCompatibilityLayerConfig.PluginGuid);
        original = AccessTools.Method(AccessTools.TypeByName("GUIClip"), "Push"); // have I ever mentioned how much I hate private/internal classes?
        prefix = AccessTools.Method(typeof(ModCompatibilityLayer), nameof(Push_Prefix));

        harmony.Patch(original, new HarmonyMethod(prefix));
      }

      Logger.LogInfo("BepinexModCompatibilityLayer finished loading successfully!");
    }

    internal static void DoWindow_Prefix(int id, GUI.WindowFunction func) {
      WindowIdSeparator.AddWindowId(id, func);
    }

    internal static void DoButton_Prefix(Rect position) {
      if (position.width > 0 && position.height > 0 && !ButtonGenerator.IgnoredRects.Contains(position) && !ButtonGenerator.GetOccupiedLocations().Any(occupiedLocation => occupiedLocation.signature == -1 && occupiedLocation.buttonFrame.Overlaps(position))) {
        ButtonGenerator.AddOccupiedLocation(position, -1);
      }
    }

    internal static void Push_Prefix(Rect screenRect) {
      if (screenRect.width > 0 && screenRect.height > 0 && !ButtonGenerator.IgnoredRects.Contains(screenRect) && !ButtonGenerator.GetOccupiedLocations().Any(occupiedLocation => occupiedLocation.signature == -1 && occupiedLocation.buttonFrame.Overlaps(screenRect))) {
        ButtonGenerator.AddOccupiedLocation(screenRect, -1);
      }
    }
  }
}