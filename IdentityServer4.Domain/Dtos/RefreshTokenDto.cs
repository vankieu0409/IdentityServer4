namespace IdentityServer4.Domain.Dtos;

public class RefreshTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}