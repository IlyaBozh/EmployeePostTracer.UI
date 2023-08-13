
namespace EmployeePostTracer.Http.Models;

public class EmployeeMainInfoResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }

    public override string ToString()
    {
        return $"{LastName.Replace(" ", "")} {FirstName.Replace(" ", "")} {Patronymic.Replace(" ", "")}";
    }
}
