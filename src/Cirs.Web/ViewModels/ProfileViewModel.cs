namespace Cirs.Web.ViewModels;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string SessionLoginTime { get; set; } = string.Empty;
    public int ProfileVisits { get; set; }
}
