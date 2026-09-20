using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Enums;

public enum OrderStatus
{
    [Display(Name = "Pending")]
    Pending = 0,

    [Display(Name = "Out for delivery")]
    OutForDelivery = 1,

    [Display(Name = "Delivered")]
    Delivered = 2,

    [Display(Name = "Rejected")]
    Rejected = 3
}
