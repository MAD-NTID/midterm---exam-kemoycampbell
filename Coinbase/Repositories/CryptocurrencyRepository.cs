using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.Exceptions;
using Coinbase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Services;

namespace Coinbase.Repositories
{
    public class CryptoCurrencyRepository: ICryptocurrencyRepository
    {
        public readonly DatabaseContext _database;

        public CryptoCurrencyRepository(DatabaseContext database)
        {
            _database = database;
        }
        public async Task<IEnumerable<Cryptocurrency>> All()
        {
           return await _database.Cryptocurrencies.ToListAsync();
        }

        public async Task<Cryptocurrency> Get(int rank)
        {
            Cryptocurrency currency = await _database.Cryptocurrencies.FirstOrDefaultAsync(item => item.Rank == rank);
            if (currency is null)
                throw new UserErrorException($"No cryptocurrency found for the rank {rank}", 404);
            return currency;
            
        }

        public async Task<Cryptocurrency> Create(Cryptocurrency cryptocurrency)
        {
            if (cryptocurrency is null)
                throw new UserErrorException("cryptocurrency cannot be null");
            
            await _database.Cryptocurrencies.AddAsync(cryptocurrency);
            await _database.SaveChangesAsync();
            return cryptocurrency;
        }

        public async Task<Cryptocurrency> Update(Cryptocurrency cryptocurrency)
        {
            _database.Cryptocurrencies.Update(cryptocurrency);
            await _database.SaveChangesAsync();
            return cryptocurrency;
        }

        public async void Delete(int rank)
        {
            Cryptocurrency currency = await Get(rank);
            _database.Cryptocurrencies.Remove(currency);
            await _database.SaveChangesAsync();
        }

        public async Task<IEnumerable<CoinMarketCap>> MarketCap()
        {
            List<CoinMarketCap> caps = new List<CoinMarketCap>();
            foreach (Cryptocurrency cryptocurrency in _database.Cryptocurrencies)
            {
                caps.Add(new CoinMarketCap(){Name = cryptocurrency.Name, MarketCap = cryptocurrency.MarketCap,
                    AvailableSupply = cryptocurrency.AvailableSupply});
            }

            return caps;
        }

        public async Task<IEnumerable<Cryptocurrency>> Search(string name)
        {
            name = name.ToLower();
            return await _database.Cryptocurrencies.Where(item => item.Name.ToLower().Contains(name)).ToListAsync();
        }

        public async Task<IEnumerable<Cryptocurrency>> PriceRange(double min, double max)
        {

            List<Cryptocurrency> cryptocurrencies = new List<Cryptocurrency>();
            //loop through each of the currencies
            foreach (Cryptocurrency currency in _database.Cryptocurrencies)
            {
                //strip out the $ and , from the price string
                currency.Price = currency.Price.Replace("$", "").Replace(",", "");
                //convert the price to double so we can use it for range comparing
                double price = double.Parse(currency.Price);
                
                //if the price is in the range, we add it to the list
                if (price >= min && price <= max)
                {
                    cryptocurrencies.Add(currency);
                }
            }

            return cryptocurrencies;
            
     
        }
    }
}