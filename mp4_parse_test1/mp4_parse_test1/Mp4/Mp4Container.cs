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

		public static Mp4Container Parse(FileStream stream)
		{
			Mp4Container container = new Mp4Container();

			using (var br = new BinaryReader(stream, true))
			{
				new BoxParser(br, container).Parse();
				//container.Boxes.AddRange(new BoxParser(br, container).Parse());
			}

			return container;
		}
	}
}
