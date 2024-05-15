using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace SpiderWebFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SpiderWebFix : BaseUnityPlugin {
    public static SpiderWebFix Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    private static ConfigEntry<bool> _sendDebugMessages = null!;
    private static ConfigEntry<LogLevel> _debugMessageLogLevel = null!;
    internal static ConfigEntry<bool> teleportCobwebs = null!;

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        _sendDebugMessages = Config.Bind("1. Debug", "1. Send Debug Messages", false,
                                         "If true, will send additional information. Please turn this on, if you encounter any bugs.");

        _debugMessageLogLevel = Config.Bind("1. Debug", "2. Debug Message Log Level", LogLevel.NORMAL,
                                            "The higher the log level, the more spam. Only set this higher if asked.");

        teleportCobwebs = Config.Bind("2. Fixes", "Teleport Cobwebs", true,
                                      "If true, will teleport cobwebs somewhere else as soon as they are broken.");


        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch() {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    internal static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) {
        if (!_sendDebugMessages.Value)
            return;

        if (logLevel > _debugMessageLogLevel.Value)
            return;

        Logger.LogInfo($"[Debug] {message}");
    }
}