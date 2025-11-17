using ChatClient.Core;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChatClient.UI.Components
{
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
        public Action ResetCursorBlink { get; init; } = default!;
        public string FieldName { get; init; } = "TextField";
    }

    public class ClipboardActions
    {
        private readonly ClipboardContext ctx;

        // null checks constructor (safety checks)
        public ClipboardActions(ClipboardContext context)
        {
            ctx = context ?? throw new ArgumentNullException(nameof(context));

            // validate required members early for clearer errors
            if (ctx.GetText is null) throw new ArgumentException("GetText delegate required", nameof(context));
            if (ctx.SetText is null) throw new ArgumentException("SetText delegate required", nameof(context));
            if (ctx.InsertText is null) throw new ArgumentException("InsertText delegate required", nameof(context));
            if (ctx.SaveStateForUndo is null) throw new ArgumentException("SaveStateForUndo delegate required", nameof(context));
            if (ctx.UndoStack is null) throw new ArgumentException("UndoStack required", nameof(context));
            if (ctx.ResetCursorToStart is null) throw new ArgumentException("ResetCursorToStart delegate required", nameof(context));
            if (ctx.ResetCursorBlink is null) throw new ArgumentException("ResetCursorBlink delegate required", nameof(context));




        }
        public  void Process()
        {
            bool ctrlDown = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);
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
                        Raylib.SetClipboardText(ctx.GetText() ?? string.Empty);
                        Log.Info($"[{ctx.FieldName}] Copied to clipboard - Length: {(ctx.GetText()?.Length ?? 0)}");
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{ctx.FieldName}] Copy failed: {ex.Message}");
                    }
                    break;

                case ClipboardAction.Paste:
                    try
                    {
                        string clipboard = Raylib.GetClipboardText_();
                        if (!string.IsNullOrEmpty(clipboard))
                        {
                            ctx.SaveStateForUndo();
                            ctx.InsertText(clipboard);
                            Log.Info($"[{ctx.FieldName}] Pasted from clipboard - Text length: {clipboard.Length}");
                        }
                        else
                        {
                            Log.Info($"[{ctx.FieldName}] Clipboard empty on paste");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{ctx.FieldName}] Paste failed: {ex.Message}");
                    }
                    break;

                case ClipboardAction.Cut:
                    try
                    {
                        var current = ctx.GetText();
                        if (!string.IsNullOrEmpty(current))
                        {
                            ctx.SaveStateForUndo();
                            Raylib.SetClipboardText(current);
                            Log.Info($"[{ctx.FieldName}] Cut to clipboard - Previous text: '{current.Replace("\n", "\\n")}'");
                            ctx.SetText(string.Empty);
                            ctx.ResetCursorToStart();
                            ctx.ResetCursorBlink();
                        }
                        else
                        {
                            Log.Info($"[{ctx.FieldName}] Cut requested but field is empty");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"[{ctx.FieldName}] Cut failed: {ex.Message}");
                    }
                    break;

                case ClipboardAction.Undo:
                    if (ctx.UndoStack!.Count > 0)
                    {
                        string previous = ctx.UndoStack.Pop();
                        ctx.SetText(previous ?? string.Empty);
                        ctx.ResetCursorBlink();
                        Log.Info($"[{ctx.FieldName}] Undo performed - Current text: '{(previous ?? string.Empty).Replace("\n", "\\n")}'");
                    }
                    else
                    {
                        Log.Info($"[{ctx.FieldName}] Undo requested but no history");
                    }
                    break;

                case ClipboardAction.None:
                default:
                    break;

            }
        }


    }
}


