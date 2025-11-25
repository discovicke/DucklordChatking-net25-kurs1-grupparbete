using ChatClient.UI.Components.Base;
using Raylib_cs;

namespace ChatClient.UI.Components.Text
{
    /// <summary>
    /// Responsible for: managing tab navigation between multiple text fields.
    /// Handles Tab key press to cycle focus forward and Shift+Tab to cycle backward through registered fields.
    /// </summary>
    // Simple Tab navigation between registered TextFields
    public class TabLogics
    {
        private List<TextField> Fields { get; } = new();

        // Register fields in tab order
        public void Register(TextField field)
        {
            if (field == null)
            {
                return;
            }

            if (Fields.Contains(field))
            {
                return;
            }

            Fields.Add(field);

            // Auto-focus the first field if none focused yet
            if (Fields.Count == 1 && !field.IsFocused)
            {
                field.Focus();
            }
        }
        // clears all fields
        public void Clear() => Fields.Clear();

        public void Update()
        {
            if (!Raylib.IsKeyPressed(KeyboardKey.Tab))
            {
                return;
            }

            if (Fields.Count == 0)
            {
                return;
            }

            bool backwards = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

            int currentIndex = Fields.FindIndex(f => f.IsFocused);
            if (currentIndex < 0)
            {
                // Nothing focused yet: focus first and stop
                Fields[0].Focus();
                return;
            }

            int next = backwards
                ? (currentIndex - 1 + Fields.Count) % Fields.Count
                : (currentIndex + 1) % Fields.Count;

            if (next == currentIndex && Fields.Count == 1)
            {
                return;
            }
            Fields[currentIndex].Blur();
            Fields[next].Focus();
        }
    }
}

