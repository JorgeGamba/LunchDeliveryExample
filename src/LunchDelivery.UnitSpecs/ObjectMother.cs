using LunchDelivery.Schedule;

namespace LunchDelivery.UnitSpecs
{
    public static class ObjectMother
    {
        public static MovementDescription CreateMovementDescriptionFrom(string source)
        {
            MovementDescription.TryCreateFrom(source, out var result, out _);
            return result;
        }
    }
}