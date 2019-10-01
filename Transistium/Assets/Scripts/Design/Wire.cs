
namespace Transistium.Design
{
	public struct Wire
	{
		public Junction a, b;


		public bool Connects(Junction a, Junction b)
		{
			return (this.a == a && this.b == b) || (this.a == b && this.b == a);
		}

	}
}
