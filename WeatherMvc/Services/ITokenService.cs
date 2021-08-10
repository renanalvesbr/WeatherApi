using IdentityModel.Client;
using System.Threading.Tasks;

namespace WeatherMvc.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
