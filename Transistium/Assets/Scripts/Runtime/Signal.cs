
namespace Transistium.Runtime
{
	[System.Flags]
	public enum Signal
	{
		FLOATING = 0,
		LOW = 1,
		HIGH = 2,
	}

	public static class SignalExtensions
	{
		// Since state are bit flags, this returns false the case where the wire is both pulled high and pulled low
		public static bool ToLogicLevel(this Signal signal) => signal == Signal.HIGH;
	}

}