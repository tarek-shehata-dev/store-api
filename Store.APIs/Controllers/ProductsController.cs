using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.APIs.Attributes;
using Store.APIs.Errors;
using Store.Core.Dtos.Products;
using Store.Core.Helper;
using Store.Core.Services.Contract;
using Store.Core.Specifications.Products;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
         

        [ProducesResponseType(typeof(PaginationResponse<ProductDto>),StatusCodes.Status200OK)]
        [HttpGet] 
        [Cached(100)]
        [Authorize]
        public async Task<ActionResult<PaginationResponse<ProductDto>>> GetAllProducts([FromQuery] ProductSpecParams productSpec) 
        { 
           var result = await _productService.GetAllProductsAsync(productSpec);
           return Ok(result); 
        }

        [ProducesResponseType(typeof(IEnumerable<TypeBrandDto>), StatusCodes.Status200OK)]
        [HttpGet("types")]                
        [Authorize]
        public async Task<ActionResult<IEnumerable<TypeBrandDto>>> GetAllTypes()
        {
            var result = await _productService.GetAllTypesAsync();
            return Ok(result);
        }


        [ProducesResponseType(typeof(IEnumerable<TypeBrandDto>), StatusCodes.Status200OK)]
        [HttpGet ("brands")] 
        [Authorize]
        public async Task<ActionResult<IEnumerable<TypeBrandDto>>> GetAllBrands()
        {
            var result = await _productService.GetAllBrandsAsync();
            return Ok(result);
        }
        
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int? id)
        {
          if (id is null) return BadRequest(new ApiErrorResponse(400));
          var result = await _productService.GetProductById(id.Value);
            if (result is null) return NotFound(new ApiErrorResponse(404,$"The Product with Id : {id} not found at DB"));
          return Ok(result);
        }
    }
}
