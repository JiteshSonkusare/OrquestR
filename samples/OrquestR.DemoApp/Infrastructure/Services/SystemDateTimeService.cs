using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class SystemDateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
}