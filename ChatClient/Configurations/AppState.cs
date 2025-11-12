namespace ChatClient.Configurations
{
    public enum Screen
    {
        Start,
        Register,
        Chat
    }

    public static class AppState
    {
        public static Screen CurrentScreen { get; set; } = Screen.Start;
    }
}