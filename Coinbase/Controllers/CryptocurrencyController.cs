using System.Threading.Tasks;
using Coinbase.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coinbase.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cryptocurrencies/")]
    public class CryptocurrencyController : ControllerBase
    {
        private readonly ICryptocurrencyRepository _repository;

        public CryptocurrencyController(ICryptocurrencyRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> All()
        {
            return Ok(await _repository.All());
        }
    }
}