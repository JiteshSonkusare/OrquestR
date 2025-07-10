using OrquestR;
using Shared.Wrapper;

namespace Application.Features.Users.Queries;

public record GetAllUserQuery : IRequest<Result<List<UserDto>>>;