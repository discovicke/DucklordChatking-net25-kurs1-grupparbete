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
        public Func<string>? GetText { get; init; }
        public Action<string>? SetText { get; init; }
        public Action<string>? InsertText { get; init; }
        public Action? SaveStateForUndo { get; init; }
        public Stack<string>? UndoStack { get; init; }
        public Action? ResetCursorToStart { get; init; }
        public Action? ResetCursorBlink { get; init; }
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

    }

}
