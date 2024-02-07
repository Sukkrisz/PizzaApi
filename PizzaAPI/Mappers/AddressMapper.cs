using Data.Db.Models.Order;
using PizzaAPI.Dtos.Order;

namespace PizzaAPI.Mappers
{
    public static class AddressMapper
    {
        public static AddressModel ToModel(this AddressDto dto)
        {
            return new AddressModel(dto.City, dto.Line1, dto.Line2);
        }

        public static AddressDto ToDto(this AddressModel model)
        {
            return new AddressDto(model.City, model.Line1, model.Line2);
        }
    }
}
