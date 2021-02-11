using System.Collections.Generic;

namespace LunchDelivery
{
    public record WorkOrder : ICheckResult
    {
        public DeliveryTripRequest Request;
        public ICollection<IFailedDelivery> FailedDeliveries;
    }
}