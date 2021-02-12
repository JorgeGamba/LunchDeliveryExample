using System.Collections.Generic;

namespace LunchDelivery.Schedule
{
    public class DeliveryRouteScheduler
    {
        public static ICollection<MovementDescription> Schedule(ICollection<MovementDescription> movementDescriptions, Drone assignedDrone)
        {

            return default;
        }


        private static void GuardAgainstNoDeliveries(ICollection<MovementDescription> movementDescriptions, Drone assignedDrone)
        {
        }
    }
}