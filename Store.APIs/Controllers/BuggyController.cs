using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.APIs.Errors;
using Store.Repository.Data.Contexts;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("NotFound")]
        public async Task<IActionResult> GetNotFoundRequestError() 
        {
            var brand = await _context.Brands.FindAsync(100);

            if(brand is null)
            {
                return NotFound(new ApiErrorResponse(404));
            }
            return Ok(brand);

        }


        [HttpGet("ServerError")]
        public async Task<IActionResult> GetServerError() 
        {
            var brand = await _context.Brands.FindAsync(100);

            var brandToString = brand.ToString(); 
            
            return Ok(brand);
        }


        [HttpGet("BadRequest")]
        public IActionResult GetBadRequestError() 
        {
           return BadRequest(new ApiErrorResponse(400));
        }


        [HttpGet("BadRequest/{id}")]
        public IActionResult GetBadRequestError(int id) 
        {
            return Ok();
        }


        [HttpGet("Unauthorized")]
        public IActionResult GetUnauthorizedError()
        {
            return Unauthorized(new ApiErrorResponse(401));
        }



    }
}
