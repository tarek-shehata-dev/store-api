using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Store.APIs.Errors;
using Store.Core;
using Store.Core.Dtos.Orders;
using Store.Core.Entities.Order;
using Store.Core.Services.Contract;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if(userEmail is null) return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var address = _mapper.Map<Address>(model.shipToAddress);

            var order = await _orderService.CreateOrderAsync(userEmail, model.BasketId,model.DeliveryMethodId,address);

            if (order is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }


        [Authorize]
        [HttpGet]
        public  async Task<IActionResult> GetOrdersForSpecificUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail is null) return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var orders = await _orderService.GetOrdersForSpecificUserAsync(userEmail);

            if (orders is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(_mapper.Map<IEnumerable<OrderToReturnDto>>(orders));
        }


        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrdersByIdForSpecificUser(int orderId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail is null) return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));

            var order = await _orderService.GetOrdersByIdForSpecificUserAsync(userEmail, orderId);

            if (order is null) return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound));

            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }


        [HttpGet("DeliveryMethod")]
        public  async Task<IActionResult> GetDeliveryMethod()
        {
           var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod,int>().GetAllAsync();
           if (deliveryMethod is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

           return Ok(deliveryMethod);
        }
    }
}
