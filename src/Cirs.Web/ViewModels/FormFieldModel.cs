namespace Cirs.Web.ViewModels;

// Input for the shared _FormField partial, so every field on every form is laid out the same way.
public class FormFieldModel
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string InputType { get; set; } = "text";
    public string? Value { get; set; }
    public bool Required { get; set; }
    public string Help { get; set; } = string.Empty;
    public string? Autocomplete { get; set; }

    // Name of an icon from Icons.cs shown inside the input, for example "mail".
    public string? Icon { get; set; }

    // Shows the live strength bar and rule checklist (sign up password only).
    public bool ShowStrength { get; set; }
}
