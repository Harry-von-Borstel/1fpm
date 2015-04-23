namespace blueshell.rfc822
{
	public class GroupAddress : ArgumentBase
	{
		public GroupAddress()
			: this(null)
		{
		}

		public GroupAddress(string argument)
			: base(
				argument,
				Re.GROUP
				)
		{
		}
	}
}