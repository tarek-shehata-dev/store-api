using Store.Core.Dtos.Bakets;
using Store.Core.Entities;
using Store.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<CustomerBasketDto> CreatOrUpdatePaymentIntentAsync(string basketId);

        Task<Order> UpdatePaymentIntentForSuccessOrFailed(string paymentIntentId, bool flag);
    }
}
