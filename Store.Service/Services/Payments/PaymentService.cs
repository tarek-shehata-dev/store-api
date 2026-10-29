using Microsoft.Extensions.Configuration;
using Store.Core;
using Store.Core.Dtos.Bakets;
using Store.Core.Entities;
using Store.Core.Entities.Order;
using Store.Core.Services.Contract;
using Store.Core.Specifications.Orders;
using Store.Repository;
using Store.Service.Services.Baskets;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Store.Core.Entities.Product;

namespace Store.Service.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketService _basketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketService basketService, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _basketService = basketService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<CustomerBasketDto> CreatOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var basket = await _basketService.GetBasketAsync(basketId);

            if (basket is null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod,int>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
            }


            if(basket.Items.Count() > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product, int>().GetAsync(item.Id);
                    if (item.Price != product.Price)
                    {
                        item.Price = product.Price;
                    }
                }
            }

            var subTotal = basket.Items.Sum(I => I.Price *  I.Quantity);

            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100),
                    AllowedPaymentMethodTypes = new List<string> { "card" },
                    Currency = "usd"
                };

                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100),
                };

                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

                
            basket = await _basketService.UpdateBasketAsync(basket);

            if (basket is null) return null;

            return basket;
        }
        async Task<Order> IPaymentService.UpdatePaymentIntentForSuccessOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new OrderSpecificationWithPaymentIntentId(paymentIntentId);
            var order = await _unitOfWork.Repository<Order, int>().GetWithSpecAsync(spec);

            if (order is null) return null;

            if (flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Order, int>().Update(order);

            await _unitOfWork.CompleteAsync();

            return order; 
        }
    }
}
