
namespace EmployeePostTracer.Http.Models;

public class RegistrationEmployeeRequest : UpdateEmployeeRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

