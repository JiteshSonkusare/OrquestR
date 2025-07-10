using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;

namespace Application.Features.Users;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	private readonly IRepositoryAsync<User, Guid> _repository;

	public UpdateUserCommandValidator(IRepositoryAsync<User, Guid> repository)
	{
		_repository = repository;

		RuleFor(u => u.Name)
			.NotEmpty()
			.WithMessage("Name should not be null/empty.")
			.MustAsync(async (model, name, cancellation) =>
			{
				// Ensure that name is unique for others, excluding the current user's ID
				return !await _repository.Entities
					.AnyAsync(user => user.Name == name && user.Id != model.Id, cancellation);
			})
			.WithMessage("Name must be unique.");
	}
}