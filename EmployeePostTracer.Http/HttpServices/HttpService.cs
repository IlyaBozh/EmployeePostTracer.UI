
using EmployeePostTracer.Http.HttpServices.Interfaces;
using EmployeePostTracer.Http.Models;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeePostTracer.Http.HttpServices;

public class HttpService : IHttpEmployeeService 
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly JsonSerializerOptions _options;
    private string _jwt;

    public HttpService()
    {
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7004/swagger/v1/swagger.json");
        }

        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task DeleteAccount()
    {
        var id = await GetEmployeeIdFromToken();

        await SetUpDeleteRequest($"/Employee/{id}");

        _jwt = null;
    }

    public async Task DeleteLetter(int id) => await SetUpDeleteRequest($"/Letter/{id}");


    public async Task<List<EmployeeMainInfoResponse>> GetAll() => await SetUpGetRequest<List<EmployeeMainInfoResponse>>("/Employee");


    public async Task<List<LetterMainInfoResponse>> GetAllByRecipientId()
    {
        var id = await GetEmployeeIdFromToken();

        return await SetUpGetRequest<List<LetterMainInfoResponse>>($"/employee/{id}/letters/recipient");
    }


    public async Task<List<LetterMainInfoResponse>> GetAllBySenderId()
    {
        var id = await GetEmployeeIdFromToken();

        return await SetUpGetRequest<List<LetterMainInfoResponse>>($"/employee/{id}/letters/sender");
    }


    public async Task<EmployeeAllInfoResponse> GetById()
    {
        var id = await GetEmployeeIdFromToken(); 

        return await SetUpGetRequest<EmployeeAllInfoResponse>($"/Employee/{id}");
    }


    public async Task<LetterAllInfoResponse> GetLetterById(int id) => await SetUpGetRequest<LetterAllInfoResponse>($"/Letter/{id}");
    

    public async Task Login(LoginRequest request)
    {
        HttpResponseMessage response;
        response = await SetUpPostRequest<LoginRequest>(request, "/Auth");

        _jwt = await response.Content.ReadAsStringAsync();
    }


    public async Task<int> Register(RegistrationEmployeeRequest request)
    {
        HttpResponseMessage response;
        response = await SetUpPostRequest<RegistrationEmployeeRequest>(request, "/Employee");

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<int>(content, _options);
        return result;
    }


    public async Task<int> SendLetter(AddLetterRequest requestEmployee)
    {
        var employee = await GetById();
        requestEmployee.SenderId = await GetEmployeeIdFromToken();
        requestEmployee.Sender = $"{employee.LastName.Replace(" ","")} {employee.FirstName.Replace(" ", "")} {employee.Patronymic.Replace(" ", "")}";

        var json = JsonSerializer.Serialize(requestEmployee, _options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "/Letter") 
        { 
            Content = content
        };

        if (!string.IsNullOrEmpty(_jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<int>(responseContent, _options);
        return result;
    }


    public async Task UpdateAccount(UpdateEmployeeRequest requestEmployee)
    {
        var id = await GetEmployeeIdFromToken();

        await SetUpPutRequest<UpdateEmployeeRequest>(requestEmployee, $"/Employee/{id}");
    }

    public async Task EditLetter(UpdateLetterRequest requestLetter, int id) => await SetUpPutRequest<UpdateLetterRequest>(requestLetter, $"/Letter/{id}");
    

    private async Task<int> GetEmployeeIdFromToken()
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_jwt);

        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        return 0;
    }


    private async Task SetUpPutRequest<K>(K body, string urlPath)
    {
        var json = JsonSerializer.Serialize(body, _options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, urlPath)
        {
            Content = content
        };

        if (!string.IsNullOrEmpty(_jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }


    private async Task<K> SetUpGetRequest<K>(string urlPath)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, urlPath);

        if (!string.IsNullOrEmpty(_jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<K>(content, _options);
        return result;
    }


    private async Task<HttpResponseMessage> SetUpPostRequest<T>(T body, string urlPath)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        response = await _httpClient.PostAsync(urlPath, content);

        response.EnsureSuccessStatusCode();

        return response;
    }


    private async Task SetUpDeleteRequest(string urlPath)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, urlPath);

        if (!string.IsNullOrEmpty(_jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
