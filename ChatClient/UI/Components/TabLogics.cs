using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.UI.Components
{
    public class TabLogics
    {
        private readonly List<TextField> fields = new();


        // Register a field in tab order (first registered gets initial focus)
        public void Register(TextField field)
        {
            if (field == null) return;
            if (fields.Contains(field)) return;

            fields.Add(field);

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
                return;

            if (fields.Count == 0)
                return;

            bool backwards = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

            int currentIndex = fields.FindIndex(f => f.IsFocused);
            if (currentIndex < 0)
            {
                // Nothing focused yet: focus first and stop
                fields[0].Focus();
                return;
            }

            int nextIndex = backwards
                ? (currentIndex - 1 + fields.Count) % fields.Count
                : (currentIndex + 1) % fields.Count;

            if (nextIndex == currentIndex && fields.Count == 1)
                return; // Only one field, nothing to switch

            var current = fields[currentIndex];
            var next = fields[nextIndex];

            current.Blur();
            next.Focus();
        }
    }
}

