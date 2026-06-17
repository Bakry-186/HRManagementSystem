using AutoMapper;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IEmployeeRepository repository, IMapper mapper)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponseDto>
{
    public async Task<EmployeeResponseDto> Handle(
        GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Id} was not found.");

        return mapper.Map<EmployeeResponseDto>(employee);
    }
}
