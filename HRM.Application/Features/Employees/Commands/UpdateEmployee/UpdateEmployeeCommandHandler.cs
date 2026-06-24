using AutoMapper;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IEmployeeRepository repository, IMapper mapper)
    : IRequestHandler<UpdateEmployeeCommand, EmployeeResponseDto>
{
    public async Task<EmployeeResponseDto> Handle(
        UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Id} was not found.");

        mapper.Map(request.Dto, employee);
        var updated = await repository.UpdateAsync(employee);

        return mapper.Map<EmployeeResponseDto>(updated);
    }
}
