namespace Cirs.Web.Validators;

// Collects errors keyed by field name. An empty key means a form-level error.
public class FormValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyDictionary<string, List<string>> Errors => _errors;

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var list))
        {
            list = new List<string>();
            _errors[field] = list;
        }

        if (!list.Contains(message))
        {
            list.Add(message);
        }
    }

    public bool HasError(string field) => _errors.ContainsKey(field);

    // Flat list used for the log line, for example "Email: Email is required."
    public IEnumerable<string> AllMessages() =>
        _errors.SelectMany(pair => pair.Value.Select(message =>
            string.IsNullOrEmpty(pair.Key) ? message : $"{pair.Key}: {message}"));
}
