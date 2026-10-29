using Store.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail,string basketId, int deliveryMethodId, Address shippingAddress);

        Task<IEnumerable<Order>?> GetOrdersForSpecificUserAsync(string buyerEmail);
        Task<Order?> GetOrdersByIdForSpecificUserAsync(string buyerEmail, int orderId);


    }
}
