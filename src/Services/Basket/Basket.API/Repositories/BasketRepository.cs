using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache cache;
        public BasketRepository(IDistributedCache _cache)
        {
            cache = _cache;
        }
        public async Task DeleteBasket(string userName)
        {
            await cache.RemoveAsync(userName);
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await cache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart cart)
        {
            await cache.SetStringAsync(cart.UserName, JsonConvert.SerializeObject(cart));
            return await GetBasket(cart.UserName);
        }
    }
}
