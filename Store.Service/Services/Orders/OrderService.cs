using Store.Core;
using Store.Core.Entities;
using Store.Core.Entities.Order;
using Store.Core.Services.Contract;
using Store.Core.Specifications.Orders;
using Store.Repository;
using Store.Service.Services.Baskets;
using Store.Service.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketService _basketService; 
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IBasketService basketService, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketService = basketService;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketService.GetBasketAsync(basketId);

            if (basket is null || basket.Items is null || !basket.Items.Any()) return null;

            var orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product, int>().GetAsync(item.Id);

                if (product is null) return null;
                if (item.Quantity <= 0) return null;

                var productOrderItem = new ProductItemOrder(product.Id, product.Name, product.PictureURL);
                orderItems.Add(new OrderItem(productOrderItem, product.Price, item.Quantity));
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetAsync(deliveryMethodId);

            if (deliveryMethod is null) return null;

            var subTotal = orderItems.Sum(I => I.Price * I.Quantity);


            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var spec = new OrderSpecificationWithPaymentIntentId(basket.PaymentIntentId);
                var existOrder = await _unitOfWork.Repository<Order, int>().GetWithSpecAsync(spec);
                if (existOrder is not null)
                {
                    _unitOfWork.Repository<Order, int>().Delete(existOrder);
                }
                   
            }

            var basketDto = await _paymentService.CreatOrUpdatePaymentIntentAsync(basketId);

            if (basketDto is null) return null;

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basketDto.PaymentIntentId);

            await _unitOfWork.Repository<Order, int>().AddAsync(order);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return null;

            return order;

        }

        public async Task<Order?> GetOrdersByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);
            var order = await _unitOfWork.Repository<Order, int>().GetWithSpecAsync(spec);
            if(order is null) return null;

            return order;
        }

        public async Task<IEnumerable<Order>?> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);

            var orders = await _unitOfWork.Repository<Order, int>().GetAllWithSpecAsync(spec);

            if(orders is null) return null;

            return orders;

        }
    }
}
