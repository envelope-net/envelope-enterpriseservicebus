using Envelope.EnterpriseServiceBus.ErrorHandling.Internal;
using Envelope.Exceptions;
using Envelope.ServiceBus.Configuration;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

public class ErrorHandlerConfiguration : IErrorHandlerConfiguration, IValidable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	
	public Dictionary<int, TimeSpan> IterationRetryTable { get; set; } //Dictionary<IterationCount, TimeSpan>
	IReadOnlyDictionary<int, TimeSpan> IErrorHandlerConfiguration.IterationRetryTable { get => IterationRetryTable; set => IterationRetryTable = new Dictionary<int, TimeSpan>(value); }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public TimeSpan? DefaultRetryInterval { get; set; }

	public int? MaxRetryCount { get; set; }

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.If(DefaultRetryInterval <= TimeSpan.Zero)
			.If(MaxRetryCount < 0)
			;

		return validationBuilder.Build();
	}
	public IErrorHandlingController BuildErrorHandlingController()
	{
		var error = string.Join(" | ", Validate(nameof(ErrorHandlerConfiguration))?.Select(x => x.ToString()) ?? Array.Empty<string>());
		if (!string.IsNullOrWhiteSpace(error))
			throw new ConfigurationException(error);

		var errorHandlingController = new ErrorHandlingController
		{
			IterationRetryTable = IterationRetryTable,
			DefaultRetryInterval = DefaultRetryInterval,
			MaxRetryCount = MaxRetryCount
		};

		return errorHandlingController;
	}
}
