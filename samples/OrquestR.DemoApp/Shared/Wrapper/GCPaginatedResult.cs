namespace Shared.Wrapper
{
	public class GCPaginatedResult<T> : Result<List<T>>
	{
		public int? PageNumber { get; private set; }
		public int? PageSize { get; private set; }
		public long? TotalCount { get; private set; }
		public int? TotalPages { get; private set; }

		public bool HasPreviousPage => PageNumber.HasValue && PageNumber.Value > 1;
		public bool HasNextPage => PageNumber.HasValue && TotalPages.HasValue && PageNumber.Value < TotalPages.Value;

		protected GCPaginatedResult(List<T>? data, bool succeeded, Error? error, long? count, int? page, int? pageSize)
			: base(data, succeeded, error)
		{
			PageNumber = page;
			PageSize = pageSize;
			TotalCount = count;
			TotalPages = (pageSize > 0 && count.HasValue && pageSize.HasValue)
				? (int)Math.Ceiling(count.Value / (double)pageSize.Value)
				: null;
		}

		public static GCPaginatedResult<T> Success(List<T> data, long? count, int? page, int? pageSize)
		{
			return new GCPaginatedResult<T>(data, true, null, count, page, pageSize);
		}

		public static new GCPaginatedResult<T> Failure(Error error)
		{
			return new GCPaginatedResult<T>(new List<T>(), false, error, null, null, null);
		}
	}
}
