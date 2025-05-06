using AutoMapper;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Models;


namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var configs = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return configs;
        }
    }
}
