namespace Revelation.Front.Office.Models.Common
{
    public enum BookingStatus
    {
        Tentative = 0,
        Confirmed = 1,
        CheckedIn = 2,
        CheckedOut = 3,
        Cancelled = 4,
        NoShow = 5
    }

    public enum RoomStatus
    {
        Available = 0,
        Occupied = 1,
        Reserved = 2,
        OutOfOrder = 3,
        UnderMaintenance = 4
    }

    public enum HousekeepingStatus
    {
        Dirty = 0,
        Clean = 1,
        Inspected = 2,
        Occupied = 3
    }

    public enum PaymentStatus
    {
        Unpaid = 0,
        PartiallyPaid = 1,
        Paid = 2,
        Refunded = 3
    }

    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        Cash = 2,
        BankTransfer = 3,
        OnlinePayment = 4
    }

    public enum BookingSource
    {
        WalkIn = 0,
        Website = 1,
        Phone = 2,
        TravelAgency = 3,
        OTA = 4
    }

    public enum MaintenancePriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum MaintenanceStatus
    {
        Reported = 0,
        Assigned = 1,
        InProgress = 2,
        OnHold = 3,
        Completed = 4,
        Cancelled = 5
    }

    public enum MaintenanceCategory
    {
        Electrical = 0,
        Plumbing = 1,
        AirConditioning = 2,
        Furniture = 3,
        Appliances = 4,
        InternetNetwork = 5,
        Structural = 6,
        GeneralMaintenance = 7
    }

    public enum IdentificationType
    {
        Passport = 0,
        DriversLicense = 1,
        NationalId = 2
    }
}
