    using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.APIs.Errors;
using Store.Core.Dtos.Bakets;
using Store.Core.Entities;
using Store.Core.Repositories.Contract;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string? id)
        {
            if (id is null)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid  Id !!"));
            }

            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket() { Id = id });

        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto model)
        {

            var basket = await _basketRepository.UpdateBasketAsync(_mapper.Map<CustomerBasket>(model));

            if (basket is null)
            {
                return BadRequest(new ApiErrorResponse(400));
            }

            return Ok(basket);

        }

        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
             await _basketRepository.DeleteBasketAsync(id);
        }

    }
}
