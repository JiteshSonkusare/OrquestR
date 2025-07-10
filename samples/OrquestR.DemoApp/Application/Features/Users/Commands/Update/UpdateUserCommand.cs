using OrquestR;
using Shared.Wrapper;

namespace Application.Features.Users;

public record UpdateUserCommand(
	Guid Id,
	string? Name,
	string? Email,
	string? Address,
	string? City,
	string? Phone) 
: IRequest<Result<Guid>>;

public record UpdateUserRequest(
	string? Name,
	string? Email,
	string? Address,
	string? City,
	string? Phone);