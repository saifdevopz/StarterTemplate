using Common.Domain.Errors;

namespace Parent.Domain.Common.Errors;
public static class ItemErrors
{
    public static CustomError NotFound(Guid orderId)
    {
        return CustomError.NotFound("Events.NotFound", $"The event with the identifier {orderId} was not found");
    }

    public static readonly CustomError StartDateInPast = CustomError.Problem(
            "Events.StartDateInPast",
            "The event start date is in the past");

    public static readonly CustomError EndDatePrecedesStartDate = CustomError.Problem(
            "Events.EndDatePrecedesStartDate",
            "The event end date precedes the start date");

    public static readonly CustomError NoTicketsFound = CustomError.Problem(
            "Events.NoTicketsFound",
            "The event does not have any ticket types defined");

    public static readonly CustomError NotDraft = CustomError.Problem("Events.NotDraft", "The event is not in draft status");


    public static readonly CustomError AlreadyCanceled = CustomError.Problem(
            "Events.AlreadyCanceled",
            "The event was already canceled");


    public static readonly CustomError AlreadyStarted = CustomError.Problem(
            "Events.AlreadyStarted",
            "The event has already started");
}
