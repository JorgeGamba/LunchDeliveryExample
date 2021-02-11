namespace LunchDelivery.Delivery
{
    public record FailedByAlreadyConfirmedDelivery : IFailedDelivery
    {
        public ConfirmedDelivery AlreadyConfirmedDelivery;
        public ConfirmedDelivery ConfirmedDelivery;
        public string Reason;
    }
}