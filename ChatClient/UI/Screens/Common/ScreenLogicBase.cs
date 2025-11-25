using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;

namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Base class for screen logic with common patterns
/// </summary>
public abstract class ScreenLogicBase(INavigationService? navigationService = null) : IScreenLogic
{
    protected readonly INavigationService Navigation = navigationService ?? new NavigationService();
    private readonly FieldGroup Fields = new();
    private bool isInitialized;

    public void HandleInput()
    {
        if (!isInitialized)
        {
            Initialize();
            isInitialized = true;
        }

        UpdateComponents();
        HandleActions();
    }

    /// <summary>
    /// Called once to initialize the screen
    /// </summary>
    protected void Initialize()
    {
        Fields.Initialize();
    }

    /// <summary>
    /// Update all UI components
    /// </summary>
    protected virtual void UpdateComponents()
    {
        Fields.UpdateAll();
    }

    /// <summary>
    /// Handle user actions (button clicks, etc.)
    /// </summary>
    protected abstract void HandleActions();

    /// <summary>
    /// Register a field for management
    /// </summary>
    protected void RegisterField(TextField field)
    {
        Fields.AddField(field);
    }

    /// <summary>
    /// Clear all registered fields
    /// </summary>
    protected void ClearFields()
    {
        Fields.ClearAll();
    }
}

