namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

public enum BodyType
{
	Root,
	Inline,
	If,
	IfElse,
	Switch,
	While,
	Parallel,
	WaitForEvent,
	Delay,
	Custom
}
