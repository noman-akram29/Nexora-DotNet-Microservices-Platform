using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Request;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Role name is required.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Danh sách PermissionId gán cho Role này.</summary>
    public List<Guid> PermissionIds { get; set; } = new();
}
