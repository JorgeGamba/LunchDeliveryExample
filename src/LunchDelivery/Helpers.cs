using System;

namespace LunchDelivery
{
    public static class Helpers
    {
        public static string MergeFailureReasons(string formerFailureReason, string additionalFailureReason) =>
            formerFailureReason is null
                ? additionalFailureReason
                : additionalFailureReason is null
                    ? null
                    : formerFailureReason + Environment.NewLine + additionalFailureReason;
    }
}