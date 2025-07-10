using OrquestR;
using LazyCache;
using Shared.Wrapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Application.Common.ExceptionHandlers;

namespace Application.Features.Users.Queries;

internal class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Result<List<UserDto>>>
{
	private readonly IUnitOfWork<Guid> _unitOfWork;
	private readonly IAppCache _cache;

	public GetAllUserQueryHandler(IUnitOfWork<Guid> unitOfWork, IAppCache cache)
	{
		_unitOfWork = unitOfWork;
		_cache = cache;
	}

	public async Task<Result<List<UserDto>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
	{
		try
		{
			Task<List<User>> getAllUsers() => _unitOfWork.Repository<User>().GetAllAsync();
			var users = await _cache.GetOrAddAsync(CacheConstants.UserCacheKey, getAllUsers);
			if (users.Count == 0)
				return Result.Failure<List<UserDto>>(UserError.NotFound);

			var mappedUser = new List<UserDto>();

			foreach (var user in users) 
			{
				var model = new UserDto
				{
					Id = user.Id,
					Name = user.Name,
					Email = user.Email,
					Address = user.Address,
					Phone = user.Phone,
					City = user.City,
					CreatedOn = user.CreatedOn,
					ModifiedOn = user.ModifiedOn
				};
				mappedUser.Add(model);
			}

			return Result.Success(mappedUser);
		}
		catch (Exception ex)
		{
			throw ex.With("Failed to get user data!", ex.Message);
		}
	}
}
