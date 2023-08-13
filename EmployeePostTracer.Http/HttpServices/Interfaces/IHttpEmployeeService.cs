
using EmployeePostTracer.Http.Models;

namespace EmployeePostTracer.Http.HttpServices.Interfaces;

public interface IHttpEmployeeService
{
    Task<int> Register(RegistrationEmployeeRequest request);
    Task Login(LoginRequest request);
    Task<EmployeeAllInfoResponse> GetById();
    Task<List<LetterMainInfoResponse>> GetAllByRecipientId();
    Task<List<LetterMainInfoResponse>> GetAllBySenderId();
    Task<int> SendLetter (AddLetterRequest request);
    Task<List<EmployeeMainInfoResponse>> GetAll();
    Task UpdateAccount(UpdateEmployeeRequest request);
    Task DeleteAccount();
    Task<LetterAllInfoResponse> GetLetterById(int id);
    Task EditLetter(UpdateLetterRequest request, int id);
    Task DeleteLetter(int id);
}
