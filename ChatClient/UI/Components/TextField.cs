using ChatClient.Core;
using ChatClient.UI.Components;
using ChatClient.UI.Rendering;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class TextField : UIComponent
    {
        // TODO Bugg FIX!!! ctrl + z should return to last text state

        // TODO:
        // - Add scroll logic
        // - Add copy/paste support (Ctrl + C / Ctrl + V)
        // - Add cut support (Ctrl + X)
        // - Add undo/redo support (Ctrl + Z / Ctrl + Y)
        // - Add text selection support (Mouse drag || shift key)
        // - Add font support

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

        // Undo stack + clipboard helper
        private readonly Stack<string> undoStack = new();
        private const int MaxUndoEntries = 100;
        private readonly ClipboardActions clipboardActions;

        private bool movedThisFrame = false;

        //tab logics 
        public bool IsFocused => IsSelected;
        public void Focus() 
        {
            IsSelected = true;
            cursor.ResetBlink();
        }
        public void Blur() 
        {
            IsSelected = false;
            cursor.ResetInvisible();
        }
        
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

            // Clipboard helper to inject delegates
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
            Log.Info($"[{FieldName}] Undo state saved - Stack size: {undoStack.Count} - State: '{currentState.Replace("\n", "\\n")}'");
        }

        // TODO: Manage corner roundness
        public override void Draw()
        {
            // Determine fill color based on state
            Color fill;
            if (IsSelected)
                fill = Colors.TextFieldSelected;
            else if (MouseInput.IsHovered(Rect))
                fill = Colors.TextFieldHovered;
            else
                fill = Colors.TextFieldUnselected;

            // Draw background
            Raylib.DrawRectangleRounded(Rect, 0.1f, 10, fill);

            // Draw border/outline
            if (IsSelected || MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.1f, 10, 2, Colors.OutlineColor);
            }
            else
            {
                // Subtle outline when not selected/hovered
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.1f, 10, 1, Colors.OutlineColor);
            }

            // Draw text or placeholder
            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(PlaceholderText))
            {
                // Draw placeholder text (always shown when text is empty) - smaller font size
                float textX = Rect.X + 4;
                float textY = Rect.Y + 4;
                Raylib.DrawTextEx(ResourceLoader.RegularFont, PlaceholderText,
                    new System.Numerics.Vector2(textX, textY), 18, 0.5f, Colors.PlaceholderText);
            }

            // Always draw actual text and cursor if there is text or field is selected
            if (!string.IsNullOrEmpty(Text) || IsSelected)
            {
                renderer.Draw(Text, cursor, IsSelected);
            }
        }

        public override void Update()
        {
            if (MouseInput.IsLeftClick(Rect))
            {
                if (!IsSelected)
                {
                    Log.Info($"[{FieldName}] Field selected");
                }
                IsSelected = true;
                cursor.ResetBlink();
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !MouseInput.IsHovered(Rect))
            {
                if (IsSelected)
                {
                    Log.Info($"[{FieldName}] Field deselected - Final text: '{Text}'");
                }
                IsSelected = false;
                cursor.ResetInvisible();
            }

            if (!IsSelected)
            { 
                return; 
            }

            
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

            // Normal character input
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

            // Backspace handling (single press / repeat)
            bool backspacePressed = (Raylib.IsKeyPressed(KeyboardKey.Backspace) && !backspaceHandledThisFrame)
                                    || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace);

            if (backspacePressed)
            {
                DeleteCharacter();
                backspaceHandledThisFrame = true;
            }
            else if (!backspacePressed)
            {
                backspaceHandledThisFrame = false;
            }

        }
        private bool WasLastCharWhitespace()
        {
            if (string.IsNullOrEmpty(Text) || cursor.Position <= 0)
            {
                return false;
            }
            int idx = Math.Min(cursor.Position - 1, Text.Length - 1);
            return char.IsWhiteSpace(Text[idx]);
        }

        //  Navigation for arrow keys
        private bool TryPress(KeyboardKey key, Action action)
        {
            if (movedThisFrame) // block duplicate action within same press/frame
                return false;

            if (Raylib.IsKeyPressed(key))
            {
                action();
                movedThisFrame = true;
                return true;
            }
            return false;
        }
        // Method for arrow Navigation with Lamda
        private void HandleNavigation()
        {
            if (movedThisFrame)
            {
                return;
            }
            Log.Error($"HandleNavigation called");
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
        }

        private void DeleteCharacter()
        {
            if (cursor.Position <= 0 || Text.Length == 0)
            {
                return;
            }

            if (!isTypingWord)
            {
                SaveUndoIfChanged();
            }

            int removeIndex = Math.Clamp(cursor.Position - 1, 0, Text.Length - 1);
            char deletedChar = Text[removeIndex];
            Text = Text.Remove(removeIndex, 1);
            cursor.Position = removeIndex;
            cursor.ResetBlink();

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
        // TODO: Mouse click to get position in text
        // TODO: Crtl backspace to  delete one word
        // TODO: Font?
    }
}