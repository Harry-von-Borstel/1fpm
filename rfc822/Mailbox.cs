namespace blueshell.rfc822
{
	public class Mailbox : ArgumentBase
	{
		public Mailbox()
			: this(null)
		{
		}

		public Mailbox(string argument)
			: base(
				argument,
				Re.MAILBOX
				)
		{
		}


	}
}