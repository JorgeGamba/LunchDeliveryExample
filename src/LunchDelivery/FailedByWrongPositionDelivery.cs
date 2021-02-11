namespace LunchDelivery
{
    public record FailedByWrongPositionDelivery : IFailedDelivery
    {
        public ScheduledDelivery ScheduledDelivery;
        public ConfirmedDelivery ConfirmedDelivery;
        public string Reason;
    }
}