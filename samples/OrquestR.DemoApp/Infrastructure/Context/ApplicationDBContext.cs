using Domain.Entities;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Services;

namespace Infrastructure.Context;

public class ApplicationDBContext : DbContext
{
	private readonly IDateTimeService _dateTimeService;
	public ApplicationDBContext(DbContextOptions<
		ApplicationDBContext> options,
		IDateTimeService dateTimeService) : base(options)
	{
		_dateTimeService = dateTimeService;
	}

	public DbSet<User> Users { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
	{
		foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedOn = _dateTimeService.Now;
					break;

				case EntityState.Modified:
					entry.Entity.ModifiedOn = _dateTimeService.Now;
					break;
			}
		}

		return await base.SaveChangesAsync(cancellationToken);
	}
}
