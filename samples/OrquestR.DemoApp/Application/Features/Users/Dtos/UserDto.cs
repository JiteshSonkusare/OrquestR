namespace Application.Features.Users;

public class UserDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public string? Email { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? Phone { get; set; }
	public DateTime? ModifiedOn { get; set; }
	public DateTime? CreatedOn { get; set; }
}