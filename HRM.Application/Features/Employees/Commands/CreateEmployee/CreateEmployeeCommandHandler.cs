using AutoMapper;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using MediatR;

namespace HRM.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IEmployeeRepository repository, IMapper mapper)
    : IRequestHandler<CreateEmployeeCommand, EmployeeResponseDto>
{
    public async Task<EmployeeResponseDto> Handle(
        CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await repository.EmailExistsAsync(request.Dto.Email);
        if (emailExists)
            throw new InvalidOperationException($"An employee with email '{request.Dto.Email}' already exists.");

        var employee = mapper.Map<Employee>(request.Dto);
        var created = await repository.CreateAsync(employee);

        return mapper.Map<EmployeeResponseDto>(created);
    }
}
