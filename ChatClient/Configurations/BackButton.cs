using Raylib_cs;

namespace ChatClient.Configurations
{
    public class BackButton : Button
    {
        public BackButton(Rectangle rect)
            : base(rect, "← Back", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor)
        {
        }

        public override void Draw()
        {
            if (AppState.CanGoBack)
            {
                base.Draw();
            }
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