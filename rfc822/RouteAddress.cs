namespace blueshell.rfc822
{
	public class RouteAddress
	{
		private readonly string str;

		public RouteAddress(string address)
		{
			str = address;
			// TODO: Check syntax
		}

		public override string ToString()
		{
			return str;
		}
	}
}