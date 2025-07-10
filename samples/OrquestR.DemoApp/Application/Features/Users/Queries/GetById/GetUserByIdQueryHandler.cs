using OrquestR;
using Shared.Wrapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Application.Common.ExceptionHandlers;

namespace Application.Features.Users;

internal class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
	private readonly IUnitOfWork<Guid> _unitOfWork;

	public GetUserByIdQueryHandler(IUnitOfWork<Guid> unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.Id);
			if (user == null)
				return Result.Failure<UserDto>(UserError.NotFoundWithId(request.Id));

			var mappedUser = new UserDto() 
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

			return Result.Success(mappedUser);
		}
		catch (Exception ex)
		{
			throw ex.With($"Failed to get user data with id: {request.Id}! Error: {ex.Message}");
		}
	}
}
