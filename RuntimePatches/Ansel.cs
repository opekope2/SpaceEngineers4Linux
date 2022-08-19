using Sandbox.Game;

namespace SpaceEngineers4Linux.RuntimePatches;

public static class Ansel
{
    public static void Disable()
    {
        MyPlatformGameSettings.ENABLE_ANSEL = false;
        MyPlatformGameSettings.ENABLE_ANSEL_WITH_SPRITES = false;
    }
}