namespace blueshell.rfc822
{
	public struct Re
	{
		public struct Name
		{
// ReSharper disable MemberHidesStaticFromOuterClass
			public const string ATOM = "atom";
			public const string QUOTED_STRING = "quotedString";
			public const string WORD = "word";
			public const string PHRASE = "phrase";
			public const string MAILBOX = "mailbox";
			public const string GROUP = "group";
			public const string FIELD_NAME = "fieldName";
			public const string FIELD_BODY = "fieldBody";
			public const string FIELD = "field";
			public const string MSG_ID = "msgId";
			public const string ADDR_SPEC = "addrSpec";
// ReSharper restore MemberHidesStaticFromOuterClass
		}
		public const string SPECIALS = @"[()<>@,;:\\"".\[\]]";
		const string SPECIALS1 = @"[()<>@,;:\\""\[\]]]"; // Without "."
		public const string ATOM = @"(?<" + Re.Name.ATOM + ">[!-~-" + SPECIALS + "]+)";
		public const string QUOTED_STRING = @"(?<" + Re.Name.QUOTED_STRING + ">\x22([\x00-\x7F-[\x22\\\\x0D]]|\\[\x00-\x7F])*\x22)";
		public const string WORD = "(?<" + Re.Name.WORD + ">" + ATOM + "|" + QUOTED_STRING + ")";
		public const string PHRASE = @"(?<" + Re.Name.PHRASE + @">((^|\s+)" + WORD + @")+)";
		public const string MAILBOX = @"(?<" + Re.Name.MAILBOX + @">((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|\x22((?=[\x01-\x7f])[^\x22\\]|\\[\x01-\x7f])*\x22\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|\x22((?=[\x01-\x7f])[^\x22\\]|\\[\x01-\x7f])*\x22)@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>))";
		public const string GROUP = @"(?<" + Re.Name.GROUP + ">" + Re.PHRASE + @":\s*("+ Re.MAILBOX + @"(\s*,\s*"+Re.MAILBOX+")*)?)";
		public const string FIELD_NAME = @"(?<" + Re.Name.FIELD_NAME + ">[!-~-[:]]+)";
		public const string FIELD_BODY = @"(?<" + Re.Name.FIELD_BODY + ">.*)";
		public const string FIELD = @"(?<" + Re.Name.FIELD + ">" + Re.FIELD_NAME + @"\s*:\s*" + Re.FIELD_BODY +")" ;
		public const string MSG_ID = @"(?<" +Re.Name.MSG_ID + ">\x3C(?<" + Re.Name.ADDR_SPEC + ">.+)\x3E)"; // TODO: Use addr-spec, see http://tools.ietf.org/html/rfc822#appendix-D
	}
}