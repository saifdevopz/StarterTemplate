using Common.Application.Exceptions;
using Common.Application.Mail;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Application.Behaviors;

internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger,
    IMailService mailService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
{
    public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            // Prepare the mail request
            MailRequest mailRequest = new
            (
                to: ["saif43515@gmail.com"],
                subject: "Unhandled Exception Notification",
                body: $"An unhandled exception occurred during the handling of {typeof(TRequest).Name}: {exception.Message}",
                from: "info@saifkhan.co.za",
                displayName: "Error Notification Service"
            );

            // Send the email notification
            await mailService.SendAsync(mailRequest, cancellationToken);

            logger.LogError(exception, "Exception Handling Pipeline - Unhandled exception for {RequestName}", typeof(TRequest).Name);

            throw new StarterException(typeof(TRequest).Name, innerException: exception);
        }
    }
}
