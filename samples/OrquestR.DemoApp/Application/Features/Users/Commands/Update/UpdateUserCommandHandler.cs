using OrquestR;
using Shared.Wrapper;
using Domain.Entities;
using Application.Common.ExceptionHandlers;
using Application.Interfaces.Repositories;

namespace Application.Features.Users;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Guid>>
{
	private readonly IUnitOfWork<Guid> _unitOfWork;

	public UpdateUserCommandHandler(IUnitOfWork<Guid> unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var entity = await _unitOfWork.Repository<User>().GetByIdAsync(request.Id);
			if (entity == null)
				return Result.Failure<Guid>(UserError.NotFoundWithId(request.Id));
			else
			{
				entity.Name = request.Name ?? entity.Name;
				entity.Email = request.Email ?? entity.Email;
				entity.Address = request.Address ?? entity.Address;
				entity.City = request.City ?? entity.City;
				entity.Phone = request.Phone ?? entity.Phone;

				await _unitOfWork.Repository<User>().UpdateAsync(entity);
				await _unitOfWork.CommitAndRemoveCacheAsync(cancellationToken, CacheConstants.UserCacheKey);
				return Result.Success(request.Id);
			}
		}
		catch (Exception ex)
		{
			throw ex.With("User Update Failed! Error", ex.Message)
					.DetailData(nameof(request.Id), request.Id);
		}
	}
}