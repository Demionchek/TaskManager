namespace TaskManager.API.Application.DTOs;

public class AuthResponse
{
    public string Token {get; set;} = string.Empty;
    public string RefreshToken {get; set;} = string.Empty;
    public string Username {get; set;} = string.Empty;
    public AuthResponse(string token, string refreshToken, string username)
    {
        Token = token;
        Username = username;
        RefreshToken = refreshToken;
    }
}