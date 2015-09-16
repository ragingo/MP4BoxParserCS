using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
	/// <summary>
	/// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
	/// URIBox
	/// </summary>
	public class URIInitBox : FullBox
	{
		public List<byte> UriInitializationData { get; private set; }

		public URIInitBox() : base(BoxType.uriI)
		{
			UriInitializationData = new List<byte>();
		}
	}
}
