using AutoMapper;
using Store.Core;
using Store.Core.Dtos.Products;
using Store.Core.Entities;
using Store.Core.Helper;
using Store.Core.Services.Contract;
using Store.Core.Specifications;
using Store.Core.Specifications.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaginationResponse<ProductDto>> GetAllProductsAsync(ProductSpecParams productSpec)
        {
            var spec = new ProductSpecifications(productSpec);

            var products = await _unitOfWork.Repository<Product, int>().GetAllWithSpecAsync(spec);
            var mappedProducts = _mapper.Map<IEnumerable<ProductDto>>(products);

            var CountSpec = new ProductWithCountSpecification(productSpec);

            var Count =  await _unitOfWork.Repository<Product, int>().GetCountAsync(CountSpec);

            return new PaginationResponse<ProductDto>(productSpec.PageSize,productSpec.PageIndex, Count, mappedProducts);

        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var spec = new ProductSpecifications(id);
            var product = await _unitOfWork.Repository<Product, int>().GetWithSpecAsync(spec);
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return mappedProduct;
        }
        public async Task<IEnumerable<TypeBrandDto>> GetAllTypesAsync()
        {
            return _mapper.Map<IEnumerable<TypeBrandDto>> (await _unitOfWork.Repository<ProductType, int>().GetAllAsync());
        } 
        
        public async Task<IEnumerable<TypeBrandDto>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Repository<ProductBrand, int>().GetAllAsync();
            var mappedBrands = _mapper.Map<IEnumerable<TypeBrandDto>> (brands);
            return mappedBrands;
        }
    }
}
