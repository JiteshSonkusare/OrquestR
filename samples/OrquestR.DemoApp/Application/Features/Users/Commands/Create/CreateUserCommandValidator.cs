using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;

namespace Application.Features.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserCommandValidator(IRepositoryAsync<User, Guid> _repository)
	{
		RuleFor(u => u.Name)
			.NotEmpty()
			.WithMessage("Name should not be null/empty.")
			.MustAsync(async (name, cancellationToken) =>
			{
				bool isUnique = await _repository.Entities.AnyAsync(b => b.Name.Equals(name));
				return !isUnique;
			})
			.WithMessage("Name must be unique.");
	}
}