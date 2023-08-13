
using EmployeePostTracer.Http.HttpServices.Interfaces;
using EmployeePostTracer.Http.Models;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EmployeePostTracer.Http.HttpServices;

public class HttpEmployeeService : IHttpEmployeeService 
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly JsonSerializerOptions _options;
    private string _jwt;

    public HttpEmployeeService()
    {
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7004/swagger/v1/swagger.json");
        }

        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<EmployeeAllInfoResponse> GetById()
    {

        var id = await GetEmployeeIdFromToken();

        // Создание объекта HttpRequestMessage для настройки запроса
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Employee/{id}");

        // Добавление заголовка авторизации
        if (!string.IsNullOrEmpty(_jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EmployeeAllInfoResponse>(content, _options);
        return result;
    }

    public async Task Login(LoginRequest request)
    {
        var serializedPayload = JsonSerializer.Serialize(request);
        var requestPayload = new StringContent(serializedPayload, Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        response = await _httpClient.PostAsync("/Auth", requestPayload);

        response.EnsureSuccessStatusCode();

        _jwt = await response.Content.ReadAsStringAsync();
        /*_jwt = JsonSerializer.Deserialize<string>(content, _options);*/
    }

    public async Task<int> Register(RegistrationEmployeeRequest request)
    {
        var serializedPayload = JsonSerializer.Serialize(request);
        var requestPayload = new StringContent(serializedPayload, Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        response = await _httpClient.PostAsync("/Employee", requestPayload);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<int>(content, _options);
        return result;
    }

    private async Task<int> GetEmployeeIdFromToken()
    {
        // Декодирование JWT-токена
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_jwt);

        // Извлечение значения поля "id" из полезной нагрузки
        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        return 0;
    }
}
