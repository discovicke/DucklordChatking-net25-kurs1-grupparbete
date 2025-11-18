using ChatClient.Core;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Specialized
{
    /// <summary>
    /// Responsible for: providing a pre-configured "Back" navigation button with arrow icon.
    /// Extends Button with default styling and text for consistent back navigation across screens.
    /// </summary>
    public class BackButton(Rectangle rect)
        : Button(rect, "Back", Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor)
    {
        public override void Update()
        {
            if (AppState.CanGoBack && IsClicked())
            {
                Log.Info($"[BackButton] Navigating back from {AppState.CurrentScreen}");
                AppState.GoBack();
                Log.Info($"[BackButton] Navigated to {AppState.CurrentScreen}");
            }
        }
    }
}