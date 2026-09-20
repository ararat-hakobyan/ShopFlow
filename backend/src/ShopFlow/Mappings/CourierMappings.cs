using ShopFlow.DTOModels;
using ShopFlow.Models;

namespace ShopFlow.Mappings;

public static class CourierMappings
{
    public static CourierDto ToDto(this Courier courier)
        => new()
        {
            CourierId = courier.CourierID,
            FirstName = courier.FirstName,
            LastName = courier.LastName,
            Phone = courier.Phone,
            VehicleType = courier.VehicleType,
            IsActive = courier.IsActive
        };

    public static List<CourierDto> ToDtoList(this IEnumerable<Courier> couriers)
        => couriers.Select(ToDto).ToList();
}
