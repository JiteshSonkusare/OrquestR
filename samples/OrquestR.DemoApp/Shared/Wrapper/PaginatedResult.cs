namespace Shared.Wrapper;

public class PaginatedResult<T> : Result<List<T>>
{
	// Pagination-specific properties
	public int CurrentPage { get; private set; }
	public int TotalPages { get; private set; }
	public int TotalCount { get; private set; }
	public int PageSize { get; private set; }

	public bool HasPreviousPage => CurrentPage > 1;
	public bool HasNextPage => CurrentPage < TotalPages;

	protected PaginatedResult(List<T>? data, bool succeeded, Error? error, int count, int page, int pageSize)
		: base(data, succeeded, error) 
	{
		CurrentPage = page;
		PageSize = pageSize;
		TotalCount = count;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
	}

	// Static factory method for success
	public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
	{
		return new PaginatedResult<T>(data, true, null, count, page, pageSize);
	}

	public static new PaginatedResult<T> Failure(Error error)
	{
		return new PaginatedResult<T>(new List<T>(), false, error, 0, 1, 10);
	}
}


