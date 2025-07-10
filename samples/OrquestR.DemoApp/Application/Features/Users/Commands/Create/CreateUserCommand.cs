using OrquestR;
using Shared.Wrapper;

namespace Application.Features.Users;

public record CreateUserCommand(
	string Name,
	string? Email,
	string? Address,
	string? City,
	string? Phone)
: IRequest<Result<Guid>>;

public record CreateUserRequest(
	string Name,
	string? Email,
	string? Address,
	string? City,
	string? Phone);