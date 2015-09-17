using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	public class Mp4Container
	{
		public List<Box> Boxes { get; private set; }

		public Mp4Container()
		{
			Boxes = new List<Box>();
		}

		public static Mp4Container Parse(BinaryReader reader)
		{
			Mp4Container container = new Mp4Container();

			new BoxParser(reader, container).Parse();

			return container;
		}
	}
}
