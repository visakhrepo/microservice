using Ordering.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Contracts.Persistance
{
    internal interface IOrderRepository:IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string usenName);
    }
}
