using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Text;

namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Manages a group of text fields with tab navigation and validation
/// </summary>
public class FieldGroup
{
    private readonly List<TextField> textFields = new();
    private readonly TabLogics tabLogics = new();
    private bool isInitialized;

    public void AddField(TextField field)
    {
        textFields.Add(field);
    }

    public void Initialize()
    {
        if (isInitialized) return;

        foreach (var field in textFields)
        {
            tabLogics.Register(field);
        }

        isInitialized = true;
    }

    public void UpdateAll()
    {
        tabLogics.Update();
        foreach (var field in textFields)
        {
            field.Update();
        }
    }

    public void ClearAll()
    {
        foreach (var field in textFields)
        {
            field.Clear();
        }
    }

    public IEnumerable<TextField> GetFields() => textFields;
}

