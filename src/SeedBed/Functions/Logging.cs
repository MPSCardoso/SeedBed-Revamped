using BepInEx.Logging;

namespace SeedBed.Functions;

public static class Logging
{
    private static readonly ManualLogSource SBLogger = Logger.CreateLogSource(Plugin.modName);

    public static void LogDebug(string debug)
    {
        SBLogger.LogDebug((object)debug);
    }

    public static void LogInfo(string info)
    {
        SBLogger.LogInfo((object)info);
    }

    public static void LogWarning(string warning)
    {
        SBLogger.LogWarning((object)warning);
    }

    public static void LogError(string error)
    {
        SBLogger.LogError((object)error);
    }
}
