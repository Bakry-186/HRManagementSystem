using AutoMapper;
using HRM.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HRM.Tests.Common;

public static class MappingTestHelper
{
    public static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EmployeeMappingProfile>();
            cfg.AddProfile<DepartmentMappingProfile>();
            cfg.AddProfile<AttendanceRecordMappingProfile>();
            cfg.AddProfile<PayrollRecordMappingProfile>();
        });
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
}
