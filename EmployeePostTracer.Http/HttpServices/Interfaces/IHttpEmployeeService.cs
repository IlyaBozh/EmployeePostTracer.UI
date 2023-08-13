
using EmployeePostTracer.Http.Models;

namespace EmployeePostTracer.Http.HttpServices.Interfaces;

public interface IHttpEmployeeService
{
    Task<int> Register(RegistrationEmployeeRequest request);
    Task Login(LoginRequest request);
    Task<EmployeeAllInfoResponse> GetById();
}
