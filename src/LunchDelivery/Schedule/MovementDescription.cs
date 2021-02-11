namespace LunchDelivery.Schedule
{
    public record MovementDescription
    {
        private MovementDescription(string value)
        {
            Value = value;
        }

        public static bool TryCreateFrom(string source, out MovementDescription result, out string failureReason)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                result = default;
                failureReason = "It is required to provide a text.";
                return false;
            }

            failureReason = null;
            var textWithoutSpaces = source.Replace(" ", string.Empty);
            result = new MovementDescription(textWithoutSpaces);

            return true;
        }

        public string Value { get; }
    }
}