﻿namespace ChatClient.Core
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


        // Logged in user information
        public static string LoggedInUsername { get; set; } = string.Empty;

        // Session auth token
        public static string SessionAuthToken { get; set; } = string.Empty;


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
