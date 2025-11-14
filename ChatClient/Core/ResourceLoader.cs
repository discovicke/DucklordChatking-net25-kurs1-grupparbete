using Raylib_cs;

namespace ChatClient.Core;

// Centralized resource loader for fonts, textures, and other assets.
// Ensures resources are loaded once and properly unloaded on exit.
public static class ResourceLoader
{
    // --- Fonts ---
    public static Font ExtraLightFont { get; private set; }
    public static Font LightFont { get; private set; }
    public static Font MediumFont { get; private set; }
    public static Font RegularFont { get; private set; }
    public static Font BoldFont { get; private set; }

    // --- Textures ---
    public static Texture2D LogoTexture { get; private set; }

    private static bool isLoaded = false;

    // --- Sounds ---
    public static Sound ButtonSound { get; private set; }

    // Load all resources. Call this once at application startup after Raylib.InitWindow().
    public static void LoadAll()
    {
        if (isLoaded)
        {
            Log.Info("[ResourceLoader] Resources already loaded, skipping...");
            return;
        }

        Log.Info("[ResourceLoader] Loading all resources...");

        LoadFonts();
        LoadTextures();
        LoadSounds();

        isLoaded = true;
        Log.Success("[ResourceLoader] All resources loaded successfully");
    }

    // Unload all resources. Call this before Raylib.CloseWindow().
    public static void UnloadAll()
    {
        if (!isLoaded)
        {
            Log.Info("[ResourceLoader] No resources to unload");
            return;
        }

        Log.Info("[ResourceLoader] Unloading all resources...");

        UnloadFonts();
        UnloadTextures();
        UnloadSounds();

        isLoaded = false;
        Log.Success("[ResourceLoader] All resources unloaded successfully");
    }

    private static void LoadFonts()
    {
        List<int> chars = new List<int>();

        // Basic ASCII
        for (int i = 32; i <= 126; i++) chars.Add(i);

        // Support for ÅÄÖ etc
        for (int i = 192; i <= 255; i++) chars.Add(i);

        int[] charArray = chars.ToArray();
        int charCount = chars.Count;

        ExtraLightFont = Raylib.LoadFontEx("Resources/CascadiaCode-ExtraLight.ttf", 20, charArray, charCount);
        LightFont = Raylib.LoadFontEx("Resources/CascadiaCode-Light.ttf", 20, charArray, charCount);
        MediumFont = Raylib.LoadFontEx("Resources/CascadiaCode-Medium.ttf", 20, charArray, charCount);
        RegularFont = Raylib.LoadFontEx("Resources/CascadiaCode-Regular.ttf", 20, charArray, charCount);
        BoldFont = Raylib.LoadFontEx("Resources/CascadiaCode-Bold.ttf", 20, charArray, charCount);

        Log.Info("[ResourceLoader] Fonts loaded");
    }

    private static void LoadTextures()
    {
        LogoTexture = Raylib.LoadTexture("Resources/DuckLord1.2slim.png");
        Log.Info("[ResourceLoader] Textures loaded");
    }

    private static void LoadSounds()
    {
        ButtonSound = Raylib.LoadSound("Resources/Duckquack.mp3");
        Raylib.SetSoundVolume(ResourceLoader.ButtonSound, 0.1f);
        Log.Info("[ResourceLoader] Sounds loades");
    }

    private static void UnloadFonts()
    {
        Raylib.UnloadFont(ExtraLightFont);
        Raylib.UnloadFont(LightFont);
        Raylib.UnloadFont(MediumFont);
        Raylib.UnloadFont(RegularFont);
        Raylib.UnloadFont(BoldFont);

        Log.Info("[ResourceLoader] Fonts unloaded");
    }

    private static void UnloadTextures()
    {
        Raylib.UnloadTexture(LogoTexture);
        Log.Info("[ResourceLoader] Textures unloaded");
    }

    private static void UnloadSounds()
    {
        Raylib.UnloadSound(ButtonSound);
        Log.Info("[ResourceLoader] Sounds unloaded");
    }
}

