namespace LunchDelivery
{
    public record FailedByAlreadyConfirmedDelivery : IFailedDelivery
    {
        public ConfirmedDelivery AlreadyConfirmedDelivery;
        public ConfirmedDelivery ConfirmedDelivery;
        public string Reason;
    }
}