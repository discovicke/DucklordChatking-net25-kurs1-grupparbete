 using ChatClient.UI.Components.Base;
 using ChatClient.UI.Theme;
 using Raylib_cs;

namespace ChatClient.UI.Components.Specialized
{
    /// <summary>
    /// Responsible for: providing a pre-configured "Ducktions" (Options) button.
    /// Extends Button with default styling and text for accessing settings/options screens.
    /// </summary>
    public class OptionsButton : Button
    {
        public OptionsButton(Rectangle rect)
            : base(rect, "Options", Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor)
        {
        }


        public override void Draw()
        {
            base.Draw();
        }
    }
}