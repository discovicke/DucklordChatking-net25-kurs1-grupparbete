using ChatClient.Core.Infrastructure;
using Raylib_cs;

namespace ChatClient.UI.Components.Text
{
    /// <summary>
    /// Responsible for: handling clipboard operations (copy, paste, cut) and undo/redo functionality for text fields.
    /// Manages undo stack and processes keyboard shortcuts (Ctrl+C, Ctrl+V, Ctrl+X, Ctrl+Z, Cmd on macOS).
    /// </summary>
    // enums for commands
    public enum ClipboardAction
    {
        None,
        Copy,
        Paste,
        Cut,
        Undo
    }
    // Small DTO that groups required delegates/state for clipboard operations.
    // Keeps ClipboardActions ctor simple and the dependency surface explicit.
    public sealed record ClipboardContext
    {
        public Func<string> GetText { get; init; } = default!;
        public Action<string> SetText { get; init; } = default!;
        public Action<string> InsertText { get; init; } = default!;
        public Action SaveStateForUndo { get; init; } = default!;
        public Stack<string> UndoStack { get; init; } = default!;
        public Action ResetCursorToStart { get; init; } = default!;
        public required Action<int> ResetCursorToEnd { get; init; }
        public required Action SetMovedThisFrame { get; init; }

        public Action ResetCursorBlink { get; init; } = default!;
        public string FieldName { get; init; } = "TextField";

        // Added for word navigation
        public required Func<int> GetCursorIndex { get; init; }
        public required Action<int> SetCursorIndex { get; init; }
    }

    public class ClipboardActions
    {
        private readonly ClipboardContext Context;

        // null checks constructor (safety checks)
        public ClipboardActions(ClipboardContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // validate required members early for clearer errors
            if (Context.GetText is null) throw new ArgumentException("GetText delegate required", nameof(context));
            if (Context.SetText is null) throw new ArgumentException("SetText delegate required", nameof(context));
            if (Context.InsertText is null) throw new ArgumentException("InsertText delegate required", nameof(context));
            if (Context.SaveStateForUndo is null) throw new ArgumentException("SaveStateForUndo delegate required", nameof(context));
            if (Context.UndoStack is null) throw new ArgumentException("UndoStack required", nameof(context));
            if (Context.ResetCursorToStart is null) throw new ArgumentException("ResetCursorToStart delegate required", nameof(context));
            if (Context.ResetCursorBlink is null) throw new ArgumentException("ResetCursorBlink delegate required", nameof(context));




        }
        public  void Process()
        {
            bool ctrlDown = Raylib.IsKeyDown(KeyboardKey.LeftControl) || 
                            Raylib.IsKeyDown(KeyboardKey.RightControl) || 
                            Raylib.IsKeyDown(KeyboardKey.LeftSuper) ||  // Cmd on macOS
                            Raylib.IsKeyDown(KeyboardKey.RightSuper);
                            ;
            if (!ctrlDown) return;

            ClipboardAction action = ClipboardAction.None;
            if (Raylib.IsKeyPressed(KeyboardKey.C)) action = ClipboardAction.Copy;
            else if (Raylib.IsKeyPressed(KeyboardKey.V)) action = ClipboardAction.Paste;
            else if (Raylib.IsKeyPressed(KeyboardKey.X)) action = ClipboardAction.Cut;
            else if (Raylib.IsKeyPressed(KeyboardKey.Z)) action = ClipboardAction.Undo;

            switch (action)
            {
                case ClipboardAction.Copy:
                    try
                    {
                        Raylib.SetClipboardText(Context.GetText() ?? string.Empty);
                        Context.SetMovedThisFrame();
                        Log.Info($"[{Context.FieldName}] Copied to clipboard - Length: {(Context.GetText()?.Length ?? 0)}");
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{Context.FieldName}] Copy failed: {ex.Message}");
                    }
                    return;

                case ClipboardAction.Paste:
                    try
                    {
                        string clipboard = Raylib.GetClipboardText_();
                        if (!string.IsNullOrEmpty(clipboard))
                        {
                            Context.SaveStateForUndo();
                            Context.InsertText(clipboard);
                            Context.SetMovedThisFrame();
                            Log.Info($"[{Context.FieldName}] Pasted from clipboard - Text length: {clipboard.Length}");
                        }
                        else
                        {
                            Log.Info($"[{Context.FieldName}] Clipboard empty on paste");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{Context.FieldName}] Paste failed: {ex.Message}");
                    }
                    return;

                case ClipboardAction.Cut:
                    try
                    {
                        var current = Context.GetText();
                        if (!string.IsNullOrEmpty(current))
                        {
                            Context.SaveStateForUndo();
                            Raylib.SetClipboardText(current);
                            Log.Info($"[{Context.FieldName}] Cut to clipboard - Previous text: '{current.Replace("\n", "\\n")}'");
                            Context.SetText(string.Empty);
                            Context.ResetCursorToStart();
                            Context.SetMovedThisFrame();
                            Context.ResetCursorBlink();
                        }
                        else
                        {
                            Log.Info($"[{Context.FieldName}] Cut requested but field is empty");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{Context.FieldName}] Cut failed: {ex.Message}");
                    }
                    return;

                case ClipboardAction.Undo:
                    Log.Info($"[{Context.FieldName}] Ctrl+Z detected - Stack size: {Context.UndoStack.Count}");

                    if (Context.UndoStack.Count > 1) // ✅ Minst 2 states (current + previous)
                    {
                        // Pop current state (vi vill gå tillbaka till föregående)
                        Context.UndoStack.TryPop(out _);

                        // Peek på föregående state (utan att ta bort den)
                        if (Context.UndoStack.TryPeek(out var previousState) && previousState != null)
                        {
                            Context.SetText(previousState);
                            Context.ResetCursorToEnd(previousState.Length);
                            Context.ResetCursorBlink();
                            Context.SetMovedThisFrame();
                            Log.Success($"[{Context.FieldName}] Undo successful - Restored: '{previousState.Replace("\n", "\\n")}' - Stack: {Context.UndoStack.Count}");
                        }
                    }
                    else
                    {
                        Log.Error($"[{Context.FieldName}] Cannot undo - at initial state");
                    }
                    return;


            }
        }

        private void MoveCurserLeftByWord() 
        {
            string text = Context.GetText() ?? string.Empty;
            int index = Context.GetCursorIndex();
            if (index <= 0 || text.Length == 0)
            {
                return;
            }
            int i = index - 1;

            // Skip any whitespace directly before the cursor
            while (i > 0 && char.IsWhiteSpace(text[i]))
            {
                i--;
            }

            // Move left until start or whitespace before word

            while (i > 0 && !char.IsWhiteSpace(text[i - 1]))
            {
                i--;
            }

            Context.SetCursorIndex(i);
            Context.SetMovedThisFrame();
            Context.ResetCursorBlink();
            Log.Info($"[{Context.FieldName}] Ctrl + Left -> Cursor moved left firn {index} to {i}");

        }
        private void MoveCurserRightByWord()
        {
            string text = Context.GetText() ?? string.Empty;
            int index = Context.GetCursorIndex();

            if (index >= text.Length || text.Length == 0)
            {
                return;
            }

            int i = index;

            // Skip any whitespace directly after the cursor
            while (i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i++;
            }

            // Move right until whitespace or end
            while (i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            Context.SetCursorIndex(i);
            Context.SetMovedThisFrame();
            Context.ResetCursorBlink();
            Log.Info($"[{Context.FieldName}] Ctrl+Right -> Cursor moved from {index} to {i}");
        }

    }
}


