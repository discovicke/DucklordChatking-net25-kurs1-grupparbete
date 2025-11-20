using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using Raylib_cs;

namespace ChatClient.Core.Application;

public enum WindowMode
{
    Windowed,
    FullscreenWindowed
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
    private static int WindowedWidth { get; set; } = 1024;
    private static int WindowedHeight { get; set; } = 768;
    private static int WindowedX { get; set; } = 100;
    private static int WindowedY { get; set; } = 100;
    private static bool HasWindowedSnapshot { get; set; }

    public static void SetMode(WindowMode mode)
    {
        CurrentMode = mode;
        ApplyCurrentMode(AppState.CurrentScreen);
    }

    public static void UpdateToggles(ToggleBox windowedToggle, ToggleBox fullscreenToggle)
    {

        if (windowedToggle is null || fullscreenToggle is null)
        {
            return;
        }

        bool windowedBefore = windowedToggle.IsChecked;
        bool fullscreenBefore = fullscreenToggle.IsChecked;

        windowedToggle.Update();
        fullscreenToggle.Update();

        bool windowedChanged = windowedBefore != windowedToggle.IsChecked;
        bool fullscreenChanged = fullscreenBefore != fullscreenToggle.IsChecked;

        // Initialize toggles to match current mode (only once)
        if (windowedChanged && windowedToggle.IsChecked)
        {
            fullscreenToggle.SetChecked(false);
            RestoreWindowedMode();
            SetMode(WindowMode.Windowed);
            return;
        }

        if (fullscreenChanged && fullscreenToggle.IsChecked)
        {
            windowedToggle.SetChecked(false);
            SetMode(WindowMode.FullscreenWindowed);
            return;
        }

        if (!windowedToggle.IsChecked && !fullscreenToggle.IsChecked)
        {
            if (CurrentMode == WindowMode.Windowed)
            {
                windowedToggle.SetChecked(true);
            }
            else
            {
                fullscreenToggle.SetChecked(true);
            }
        }
        else
        {
            windowedToggle.SetChecked(CurrentMode == WindowMode.Windowed);
            fullscreenToggle.SetChecked(CurrentMode == WindowMode.FullscreenWindowed);
        }
    }

    private static void CaptureWindowedBounds()
    {
        if (!Raylib.IsWindowReady())
        {
            return;
        }

        WindowedWidth = Raylib.GetScreenWidth();
        WindowedHeight = Raylib.GetScreenHeight();

        var position = Raylib.GetWindowPosition();
        WindowedX = (int)position.X;
        WindowedY = (int)position.Y;
        HasWindowedSnapshot = true;
    }

    /// <summary>
    /// Resets the initialization state. Call this when changing screens if needed.
    /// </summary>
    private static void RestoreWindowedMode()
    {
        //Raylib.ClearWindowState(ConfigFlags.UndecoratedWindow);

        if (!HasWindowedSnapshot)
        {
            var monitorPos = Raylib.GetMonitorPosition(Raylib.GetCurrentMonitor());
            WindowedX = (int)monitorPos.X + 100;
            WindowedY = (int)monitorPos.Y + 100;
        }

        Raylib.SetWindowSize(WindowedWidth, WindowedHeight);
        Raylib.SetWindowPosition(WindowedX, WindowedY);
    }

    private static void ApplyBorderlessMode()
    {
        //Raylib.SetWindowState(ConfigFlags.UndecoratedWindow);

        int monitor = Raylib.GetCurrentMonitor();
        var monitorPos = Raylib.GetMonitorPosition(monitor);
        int width = Raylib.GetMonitorWidth(monitor);
        int height = Raylib.GetMonitorHeight(monitor);

        Raylib.SetWindowSize(width, height);
        Raylib.SetWindowPosition((int)monitorPos.X, (int)monitorPos.Y);
    }


    public static void ApplyCurrentMode(Screen screen)
    {
        if (CurrentMode == WindowMode.FullscreenWindowed)
        {
            Raylib.SetWindowState(ConfigFlags.FullscreenMode);
            Raylib.SetWindowSize(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            ApplyBorderlessMode();
            CaptureWindowedBounds();
        }
        else
        {
            Raylib.ClearWindowState(ConfigFlags.FullscreenMode);
            RestoreWindowedMode();

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
}