using System.Threading.Tasks;
using Coinbase.Exceptions;
using Coinbase.Models;
using Microsoft.EntityFrameworkCore;

namespace Coinbase.Repositories
{
    public class AuthenticationRepository: IAuthenticationRepository
    {
        private readonly DbSet<Authentication> _APIKey;

        public AuthenticationRepository(DatabaseContext databaseContext)
        {
            _APIKey = databaseContext.ApiKeys;
        }
        public async Task<Authentication> Authenticate(string key)
        {
            Authentication auth = await _APIKey.FirstOrDefaultAsync(api => api.ApiKey == key);
            if (auth is null)
                throw new UserErrorException("Invalid API Key", 401);
            return auth;
            
        }
    }
}