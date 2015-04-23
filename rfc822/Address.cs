using System.Linq;

namespace blueshell.rfc822
{
	public class Address : ArgumentBase
	{
		private ArgumentBase inner;

		public Address()
			: this(null)
		{

		}

		public Address(string argument)
			: base(
				argument,
				""
			)
		{
		}

		public override bool TrySet(string argument)
		{
			inner = new Mailbox();
			if (!inner.TrySet(argument))
			{
				inner = new GroupAddress();
				if (!inner.TrySet(argument))
				{
					inner = null;
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			return
				inner == null
					? ""
					: inner.ToString();
		}
	}
}
