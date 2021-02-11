using System.Collections.Generic;

namespace LunchDelivery.Schedule
{
    public record DroneMovement
    {
        public ICollection<DroneOperation> DroneOperations;
    }
}