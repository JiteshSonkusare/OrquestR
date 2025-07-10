using Domain.Contracts;

namespace Domain.Entities;

public class User : AuditableEntity<Guid>
{
	public string Name { get; set; } = null!;
	public string? Email { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? Phone { get; set; }
}