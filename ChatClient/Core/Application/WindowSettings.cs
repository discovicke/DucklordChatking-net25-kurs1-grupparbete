using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using Raylib_cs;

namespace ChatClient.Core.Application;

public enum WindowMode
{
    Windowed,
    Fullscreen
}

/// <summary>
/// Responsible for: managing window display modes, screen-specific window sizes, and toggle UI state.
/// Applies different windowed sizes based on current screen (576x1024 for Start/Register, 1200x720 for Chat/Options).
/// Handles synchronization between window mode and UI toggle boxes.
/// </summary>
public static class WindowSettings
{
    public static WindowMode CurrentMode { get; private set; } = WindowMode.Windowed;
    private static bool togglesInitialized = false;
    
    public static void SetMode(WindowMode mode)
    {
        CurrentMode = mode;
        ApplyCurrentMode(AppState.CurrentScreen);
    }
    
    public static void ApplyCurrentMode(Screen screen)
    {
        if (CurrentMode == WindowMode.Fullscreen)
        {
            Raylib.SetWindowState(ConfigFlags.FullscreenMode);
        }
        else
        {
            Raylib.ClearWindowState(ConfigFlags.FullscreenMode);
            
            // Apply windowed size based on screen
            switch (screen)
            {
                case Screen.Start:
                case Screen.Register:
                    Raylib.SetWindowSize(500, 750);
                    break;
                    
                case Screen.Chat:
                case Screen.Options:
                    Raylib.SetWindowSize(1200, 720);
                    break;
            }
        }
        
        Log.Info($"[WindowSettings] Applied {CurrentMode} mode for {screen} screen");
    }

    /// <summary>
    /// Updates toggle boxes and handles mode switching.
    /// Call this in your screen's update loop.
    /// </summary>
    public static void UpdateToggles(ToggleBox windowedToggle, ToggleBox fullscreenToggle)
    {
        // Initialize toggles to match current mode (only once)
        if (!togglesInitialized)
        {
            bool isWindowed = CurrentMode == WindowMode.Windowed;
            windowedToggle.SetChecked(isWindowed);
            fullscreenToggle.SetChecked(!isWindowed);
            togglesInitialized = true;
        }

        windowedToggle.Update();
        fullscreenToggle.Update();

        // Handle toggle clicks
        if (windowedToggle.IsClicked())
        {
            SetModeFromToggle(WindowMode.Windowed, windowedToggle, fullscreenToggle);
        }
        else if (fullscreenToggle.IsClicked())
        {
            SetModeFromToggle(WindowMode.Fullscreen, windowedToggle, fullscreenToggle);
        }
    }

    /// <summary>
    /// Resets the initialization state. Call this when changing screens if needed.
    /// </summary>
    public static void ResetToggleInitialization()
    {
        togglesInitialized = false;
    }

    private static void SetModeFromToggle(WindowMode mode, ToggleBox windowedToggle, ToggleBox fullscreenToggle)
    {
        bool isWindowed = mode == WindowMode.Windowed;
        windowedToggle.SetChecked(isWindowed);
        fullscreenToggle.SetChecked(!isWindowed);

        if (CurrentMode != mode)
        {
            SetMode(mode);
            Log.Info($"[WindowSettings] {mode} mode selected via toggle");
        }
    }
}

