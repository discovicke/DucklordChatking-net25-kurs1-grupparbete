using Raylib_cs;

namespace ChatClient.Configurations
{
    public class OptionsButton : Button
    {
        public OptionsButton(Rectangle rect)
            : base(rect, "← Options", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor)
        {
        }

        public override void Draw()
        {
               base.Draw();
            
        }

        public override void Update()
        {
            if (AppState.CanGoBack && IsClicked())
            {
                AppState.GoBack();
                Log.Info($"Navigated back to {AppState.CurrentScreen}");
            }
        }
    }
}
