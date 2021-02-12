namespace LunchDelivery.Schedule
{
    public class CoverageChecker
    {
        private readonly int _topLimit;
        private readonly int _bottomLimit;
        private readonly int _rightLimit;
        private readonly int _leftLimit;

        public CoverageChecker(DistrictArea districtArea, int allowedBlocksAround)
        {
            _topLimit = districtArea.BlocksHigh + allowedBlocksAround;
            _bottomLimit = 0 - allowedBlocksAround;
            _rightLimit = districtArea.BlocksWide + allowedBlocksAround;
            _leftLimit = 0 - allowedBlocksAround;
        }

        public bool CanDeliverAt(Position targetPosition) =>
            targetPosition.CoordinateY <= _topLimit
            && targetPosition.CoordinateY >= _bottomLimit
            && targetPosition.CoordinateX <= _rightLimit
            && targetPosition.CoordinateX >= _leftLimit;
    }
}