using OrquestR;
using Shared.Wrapper;

namespace Application.Features.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;