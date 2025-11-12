namespace ChatClient.Configurations
{
    public enum Screen
    {
        Start,
        Register,
        Chat,
        Options
    }

    public static class AppState
    {
        private static Stack<Screen> screenHistory = new Stack<Screen>();
        private static Screen currentScreen = Screen.Start;

        public static Screen CurrentScreen
        {
            get => currentScreen;
            set
            {
                if (currentScreen != value)
                {
                    screenHistory.Push(currentScreen);
                    currentScreen = value;
                }
            }
        }

        public static bool CanGoBack => screenHistory.Count > 0;

        public static void GoBack()
        {
            if (CanGoBack)
            {
                currentScreen = screenHistory.Pop();
            }
        }

        public static void ClearHistory()
        {
            screenHistory.Clear();
        }
    }
}