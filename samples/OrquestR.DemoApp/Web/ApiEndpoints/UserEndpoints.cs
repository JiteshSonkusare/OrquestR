using OrquestR;
using Shared.Wrapper;
using Web.ExceptionHandler;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Users;
using Application.Features.Users.Queries;

namespace Web.Endpoints;

public static class UserEndpoints
{
	public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/user").WithTags("User");

		group.MapGet("/", async (
			ISender sender) =>
			{
				var result = await sender.Send(new GetAllUserQuery());

				return result.Match(
					onSuccess: users => Results.Ok(users),
					onFailure: error => Results.NotFound(error));
			})
			.WithName("GetAllUsers")
			.WithSummary("GetAll a users")
			.WithDescription("GetAll users by name using a command handler.")
			.Produces<Result<List<UserDto>>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest);

		group.MapGet("/{id:guid}", async (
			Guid id,
			ISender sender) =>
			{
				var result = await sender.Send(new GetUserByIdQuery(id));

				return result.Match(
					onSuccess: user => Results.Ok(user),
					onFailure: error => Results.NotFound(error));
			})
			.WithName("GetUserById")
			.WithSummary("GetUserById a user")
			.WithDescription("GetUserById user by name using a command handler.")
			.Produces<Result<UserDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest);

		group.MapPost("/",
			async (ISender sender,
				   [FromBody] CreateUserRequest request) =>
			{
				var command = new CreateUserCommand(

					request.Name,
					request.Email,
					request.Address,
					request.City,
					request.Phone
				);
				var result = await sender.Send(command);

				return result.Match(
					onSuccess: data => Results.Ok(data),
					onFailure: error => error.FailureResponse());
			})
			.WithName("CreateUser")
			.WithSummary("Create a user")
			.WithDescription("Creates a user by name using a command handler.")
			.Produces<Result<Guid>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest);

		group.MapPut("/{id:guid}",
			async (ISender sender,
				  Guid id,
				  [FromBody] UpdateUserRequest request) =>
			{
				var command = new UpdateUserCommand(
					id,
					request.Name,
					request.Email,
					request.Address,
					request.City,
					request.Phone
				);
				var result = await sender.Send(command);

				return result.Match(
					onSuccess: data => Results.Ok(data),
					onFailure: error => error.FailureResponse());
			})
			.WithName("UpdateUser")
			.WithSummary("Update a user")
			.WithDescription("Update user by name using a command handler.")
			.Produces<Result<Guid>>(StatusCodes.Status400BadRequest);

		group.MapDelete("/{id:guid}", async (
			Guid id,
			ISender sender) =>
			{
				var result = await sender.Send(new DeleteUserCommand(id));

				return result.Match(
					onSuccess: id => Results.NoContent(),
					onFailure: error => Results.NotFound(error));
			})
			.WithName("deleteUser")
			.WithSummary("Delete a user")
			.WithDescription("Delete user by name using a command handler.")
			.Produces<Result<Guid>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest);

		return group;
	}
}

