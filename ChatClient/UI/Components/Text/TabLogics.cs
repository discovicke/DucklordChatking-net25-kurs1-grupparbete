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

        private readonly List<TextField> fields = new();

        // Register fields in tab order
        public void Register(TextField field)
        {
            if (field == null) return;
            if (fields.Contains(field)) return;

            fields.Add(field);

            // Auto-focus the first field if none focused yet
            if (fields.Count == 1 && !field.IsFocused)
            {
                field.Focus();
            }
        }
        // clears all fields
        public void Clear() => fields.Clear();

        public void Update()
        {
            if (!Raylib.IsKeyPressed(KeyboardKey.Tab))
            {
                return;
            }

            if (fields.Count == 0)
            {
                return;
            }

            bool backwards = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

            int currentIndex = fields.FindIndex(f => f.IsFocused);
            if (currentIndex < 0)
            {
                // Nothing focused yet: focus first and stop
                fields[0].Focus();
                return;
            }

            int next = backwards
                ? (currentIndex - 1 + fields.Count) % fields.Count
                : (currentIndex + 1) % fields.Count;

            if (next == currentIndex && fields.Count == 1)
            {
                return;
            }
            fields[currentIndex].Blur();
            fields[next].Focus();
        }
    }
}

