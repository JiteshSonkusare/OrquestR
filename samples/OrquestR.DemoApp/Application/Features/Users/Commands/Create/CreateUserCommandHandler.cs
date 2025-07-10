using OrquestR;
using Shared.Wrapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Application.Common.ExceptionHandlers;

namespace Application.Features.Users;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
	private readonly IUnitOfWork<Guid> _unitOfWork;
	public CreateUserCommandHandler(IUnitOfWork<Guid> unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var entity = new User
			{
				Id = Guid.NewGuid(),
				Name = request.Name,
				Address = request.Address,
				Phone = request.Phone,
				City = request.City,
				Email = request.Email,
			};

			await _unitOfWork.Repository<User>().AddAsync(entity);
			await _unitOfWork.CommitAndRemoveCacheAsync(cancellationToken, CacheConstants.UserCacheKey);
			return Result.Create(entity.Id);
		}
		catch (Exception ex)
		{
			throw ex.With($"User creation failed, exception: {ex.Message}");
		}
	}
}