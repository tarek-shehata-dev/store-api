using Store.Core.Dtos.Bakets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Services.Contract
{
    public interface IBasketService
    {
        Task<CustomerBasketDto?> GetBasketAsync(string basketId);
        Task<CustomerBasketDto?> UpdateBasketAsync(CustomerBasketDto basketDto);
        Task<bool> DeleteBasketAsync(string basketId);

    }
}
