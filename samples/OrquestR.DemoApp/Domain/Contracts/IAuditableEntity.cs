namespace Domain.Contracts;

public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId> { }

public interface IAuditableEntity : IEntity
{
	DateTime? ModifiedOn { get; set; }
	DateTime? CreatedOn { get; set; }
}