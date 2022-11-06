
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
		public static bool ToLogicLevel(this Signal signal) => signal == Signal.HIGH;
	}

	public static class SignalUtil
	{

		public static void Merge(Signal src, ref Signal dst)
		{
			dst = Merge(src, dst);
		}

		public static Signal Merge(Signal src, Signal dst)
		{
			// TODO: replace with lookup table
			switch (dst)
			{
				default:
				case Signal.FLOATING: return src;

				case Signal.LOW: return Signal.LOW;
				case Signal.HIGH: return src == Signal.LOW ? Signal.LOW : Signal.HIGH;
			}
		}
	}

}