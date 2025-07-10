using OrquestR;
using Shared.Wrapper;

namespace Application.Features.Users;

public record DeleteUserCommand(Guid Id) : IRequest<Result<Guid>>;