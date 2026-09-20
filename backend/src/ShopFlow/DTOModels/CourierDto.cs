namespace ShopFlow.DTOModels;

public sealed class CourierDto
{
    public int CourierId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? VehicleType { get; set; }

    public bool IsActive { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
