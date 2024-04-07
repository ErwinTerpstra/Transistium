
namespace Transistium.Runtime
{
	[System.Flags]
	public enum Signal
	{
		FLOATING = 0,
		LOW = 1,
		HIGH = 2,
	}

	public enum CurrentDirection
	{
		NONE = 0,
		FORWARD = 1,
		REVERSE = 2
	}

	public static class SignalExtensions
	{
		public static bool ToLogicLevel(this Signal signal) => signal == Signal.HIGH;

		public static Signal Invert(this Signal signal) => (Signal) (2 - ((int)signal - 1));
	}

	public static class SignalUtil
	{
		private static readonly Signal[] MergeTable =
		{
			// SRC				DST					Result
			Signal.FLOATING,	Signal.FLOATING,	Signal.FLOATING,
			Signal.FLOATING,    Signal.LOW,		    Signal.LOW,
			Signal.FLOATING,    Signal.HIGH,	    Signal.HIGH,

			Signal.LOW,			Signal.FLOATING,    Signal.LOW,
			Signal.LOW,			Signal.LOW,			Signal.LOW,
			Signal.LOW,			Signal.HIGH,		Signal.LOW,

			Signal.HIGH,		Signal.FLOATING,    Signal.HIGH,
			Signal.HIGH,        Signal.LOW,         Signal.LOW,
			Signal.HIGH,        Signal.HIGH,        Signal.HIGH,
		};

		public static void Merge(Signal src, ref Signal dst)
		{
			dst = Merge(src, dst);
		}

		public static Signal Merge(Signal src, Signal dst)
		{
			int index = ((int)src * 9) + ((int)dst * 3) + 2;
			return MergeTable[index];
		}
	}

}