using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	public static class StringUtils
	{
		public static string FromBinary(UInt32 binary)
		{
			char[] chars =
				new[] {
					(char)((binary & 0xff000000) >> 24),
					(char)((binary & 0x00ff0000) >> 16),
					(char)((binary & 0x0000ff00) >>  8),
					(char)((binary & 0x000000ff) >>  0),
				};
			return new string(chars);
		}
	}
}
