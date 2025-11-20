using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.UI.Components.Text;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Base
{
    public class TextField : UIComponent
    {
        public string Text { get; private set; } = string.Empty;
        private string FieldName { get; set; } = "TextField";
        private string PlaceholderText { get; set; } = "";

        private string lastSavedState = string.Empty;
        private bool isTypingWord = false;
        private bool IsSelected { get; set; }
        private readonly bool AllowMultiline;
        private readonly TextCursor cursor;
        private readonly TextRenderer renderer;
        private bool backspaceHandledThisFrame;

        private readonly Stack<string> undoStack = new();
        private const int MaxUndoEntries = 100;
        private readonly ClipboardActions clipboardActions;
        private bool movedThisFrame = false;

        public bool IsFocused => IsSelected;
        public void Focus() { IsSelected = true; cursor.ResetBlink(); }
        public void Blur() { IsSelected = false; cursor.ResetInvisible(); }

        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor,
            bool allowMultiline = false, bool isPassword = false, string fieldName = "TextField", string placeholderText = "")
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            AllowMultiline = allowMultiline;
            FieldName = fieldName;
            PlaceholderText = placeholderText;
            cursor = new TextCursor();
            renderer = new TextRenderer(rect, textColor, isPassword, allowMultiline);

            var ctx = new ClipboardContext
            {
                GetText = () => Text,
                SetText = s => Text = s,
                InsertText = s => InsertText(s),
                SaveStateForUndo = SaveStateForUndo,
                UndoStack = undoStack,
                ResetCursorToStart = () => cursor.Position = 0,
                ResetCursorToEnd = pos => cursor.Position = pos,
                ResetCursorBlink = () => cursor.ResetBlink(),
                SetMovedThisFrame = () => movedThisFrame = true,
                FieldName = FieldName
            };
            clipboardActions = new ClipboardActions(ctx);

            undoStack.Push(string.Empty);
            SaveStateForUndo();
        }

        private void SaveStateForUndo()
        {
            string currentState = Text ?? string.Empty;
            undoStack.Push(currentState);
            if (undoStack.Count > MaxUndoEntries)
            {
                // Keep newest MaxUndoEntries states
                var latest = undoStack.ToArray(); // newest first
                undoStack.Clear();
                for (int i = 0; i < MaxUndoEntries && i < latest.Length; i++)
                { 
                    undoStack.Push(latest[i]);
                }
            }
            Log.Info($"[{FieldName}] Undo saved ({undoStack.Count}) '{currentState.Replace("\n","\\n")}'");
        }

        public override void Draw()
        {
            Color fill = IsSelected
                ? Colors.TextFieldSelected
                : (MouseInput.IsHovered(Rect) ? Colors.TextFieldHovered : Colors.TextFieldUnselected);

            Raylib.DrawRectangleRounded(Rect, 0.1f, 10, fill);
            Raylib.DrawRectangleRoundedLinesEx(Rect, 0.1f, 10, IsSelected || MouseInput.IsHovered(Rect) ? 2 : 1, Colors.OutlineColor);

            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(PlaceholderText))
            {
                const float PlaceholderFontSize = 14f;
                const float Padding = 8f;

                float x = Rect.X + Padding;
                float y = Rect.Y + (Rect.Height - PlaceholderFontSize) / 2f;
                float maxWidth = Rect.Width - (Padding * 2);

                string display = PlaceholderText;
                var size = Raylib.MeasureTextEx(ResourceLoader.RegularFont, display, PlaceholderFontSize, 0.5f);
                while (display.Length > 0 && size.X > maxWidth)
                {
                    display = display[..^1];
                    size = Raylib.MeasureTextEx(ResourceLoader.RegularFont, display, PlaceholderFontSize, 0.5f);
                }

                Raylib.DrawTextEx(ResourceLoader.RegularFont, display, new System.Numerics.Vector2(x, y),
                    PlaceholderFontSize, 0.5f, Colors.PlaceholderText);
            }

            if (!string.IsNullOrEmpty(Text) || IsSelected)
            {
                // Pass showCursor = IsSelected
                renderer.Draw(Text, cursor, IsSelected, IsSelected);
            }
        }

        public override void Update()
        {
            movedThisFrame = false;
            if (MouseInput.IsLeftClick(Rect))
            {
                if (!IsSelected) Log.Info($"[{FieldName}] Field selected");
                IsSelected = true;
                cursor.ResetBlink();
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !MouseInput.IsHovered(Rect))
            {
                if (IsSelected) Log.Info($"[{FieldName}] Field deselected - Final text: '{Text}'");
                IsSelected = false;
                cursor.ResetInvisible();
            }

            if (!IsSelected) return;

            cursor.Update(Raylib.GetFrameTime());
            clipboardActions.Process();

            HandleTextInput();
            HandleNavigation();
        }

        private void HandleTextInput()
        {
            if (AllowMultiline && IsShiftEnterPressed())
            {
                SaveUndoIfChanged();
                InsertText("\n");
                isTypingWord = false;
                return;
            }

            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if (key >= 32)
                {
                    char c = char.ConvertFromUtf32(key)[0];
                    bool isWhitespace = char.IsWhiteSpace(c);
                    if (!isTypingWord || (isWhitespace != WasLastCharWhitespace()))
                    {
                        SaveUndoIfChanged();
                        isTypingWord = !isWhitespace;
                    }
                    InsertText(c.ToString());
                }
                key = Raylib.GetCharPressed();
            }

            bool backspacePressed = (Raylib.IsKeyPressed(KeyboardKey.Backspace) && !backspaceHandledThisFrame)
                                    || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace);

            if (backspacePressed)
            {
                DeleteCharacter();
                backspaceHandledThisFrame = true;
            }
            else
            {
                backspaceHandledThisFrame = false;
            }
        }

        private bool WasLastCharWhitespace()
        {
            if (string.IsNullOrEmpty(Text) || cursor.Position <= 0) return false;
            int idx = Math.Min(cursor.Position - 1, Text.Length - 1);
            return char.IsWhiteSpace(Text[idx]);
        }

        private bool TryPress(KeyboardKey key, Action action)
        {
            if (movedThisFrame) return false;
            if (Raylib.IsKeyPressed(key))
            {
                action();
                movedThisFrame = true;
                return true;
            }
            return false;
        }

        private void HandleNavigation()
        {
            if (movedThisFrame) return;

            TryPress(KeyboardKey.Left, () => cursor.MoveLeft(Text.Length));
            TryPress(KeyboardKey.Right, () => cursor.MoveRight(Text.Length));
            TryPress(KeyboardKey.Home, () => cursor.MoveToStart());
            TryPress(KeyboardKey.End, () => cursor.MoveToEnd(Text.Length));
        }

        private void InsertText(string s)
        {
            cursor.Position = Math.Clamp(cursor.Position, 0, Text.Length);
            Text = Text.Insert(cursor.Position, s);
            cursor.Position += s.Length;
            cursor.ResetBlink();

            string displayChar = s == "\n" ? "\\n" : s;
            Log.Info($"[{FieldName}] Text inserted: '{displayChar}' - Current text: '{Text.Replace("\n", "\\n")}'");

            // Typing Sound
            Raylib.PlaySound(ResourceLoader.TypingSound);

        }

        private void DeleteCharacter()
        {
            if (cursor.Position <= 0 || Text.Length == 0) return;

            if (!isTypingWord) SaveUndoIfChanged();

            int removeIndex = Math.Clamp(cursor.Position - 1, 0, Text.Length - 1);
            char deletedChar = Text[removeIndex];
            Text = Text.Remove(removeIndex, 1);
            cursor.Position = removeIndex;
            cursor.ResetBlink();

            // Backspace sound
            Raylib.PlaySound(ResourceLoader.BackspaceSound);

            isTypingWord = true;
            Log.Info($"[{FieldName}] Deleted: '{deletedChar}' at position {removeIndex}");
        }

        private void SaveUndoIfChanged()
        {
            if (Text != lastSavedState)
            {
                SaveStateForUndo();
                lastSavedState = Text;
            }
        }

        private bool IsShiftEnterPressed()
        {
            return (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                   && Raylib.IsKeyPressed(KeyboardKey.Enter);
        }

        public void SetRect(Rectangle rect)
        {
            Rect = rect;
            renderer.UpdateBounds(rect);
        }

        public void Clear()
        {
            Log.Info($"[{FieldName}] Field cleared - Previous text: '{Text.Replace("\n", "\\n")}'");
            Text = string.Empty;
            cursor.Reset();
        }
    }
}
