
namespace Transistium.Runtime
{
	[System.Flags]
	public enum WireState
	{
		FLOATING = 0,
		LOW = 1,
		HIGH = 2,
	}

	public struct Wire
	{
		public WireState state;

		public bool LogicLevel
		{
			get
			{
				// Since state are bit flags, this returns false the case where the wire is both pulled high and pulled low
				return state == WireState.HIGH;
			}
		}
	}

}