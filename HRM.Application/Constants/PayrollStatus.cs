namespace HRM.Application.Constants;

public static class PayrollStatus
{
    public const string Draft = "Draft";
    public const string PendingApproval = "Pending Approval";
    public const string Approved = "Approved";
    public const string Paid = "Paid";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Draft, PendingApproval, Approved, Paid, Cancelled];
}
