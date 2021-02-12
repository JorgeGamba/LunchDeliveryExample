using System.Collections.Generic;

namespace LunchDelivery.Delivery
{
    public record WorkOrder : ICheckResult
    {
        public DeliveryTripRequest Request;
        public ICollection<IFailedDelivery> FailedDeliveries;
    }
}