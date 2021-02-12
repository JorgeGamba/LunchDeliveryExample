namespace LunchDelivery.Schedule
{
    public class DroneOperationFactory
    {
        public static bool TryCreateFrom(char source, out DroneOperation result, out string failureReason)
        {
            result = source switch
            {
                'A' or 'a' => DroneOperation.Advance,
                'I' or 'i' => DroneOperation.TurnLeft,
                'D' or 'd' => DroneOperation.TurnRight,
                _ => default
            };
            if (result == default)
            {
                failureReason = $"The character '{source}' is unknown.";
                return false;
            }

            failureReason = null;
            return true;
        }
    }
}