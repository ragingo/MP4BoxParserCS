using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public class DecoderConfigDescriptor : BaseDescriptor
	{
		public ObjectType ObjectTypeIndication { get; set; }
		public byte StreamType { get; set; } // 6 bits
		public byte UpStream { get; set; } // 1 bit
		public byte Reserved { get; set; } // 1 bit
		public UInt32 BufferSizeDB { get; set; } // 24 bits
		public UInt32 MaxBitrate { get; set; }
		public UInt32 AvgBitrate { get; set; }
		public DecoderSpecificInfo[] DecoderSpecificInfos { get; set; }
		public ProfileLevelIndicationIndexDescriptor[] ProfileLevelIndicationIndexDescriptors { get; set; }

		public DecoderConfigDescriptor()
		{
			Tag = DescriptorTag.DecoderConfigDescrTag;
			Reserved = 1;
			DecoderSpecificInfos = new DecoderSpecificInfo[2];
			ProfileLevelIndicationIndexDescriptors = new ProfileLevelIndicationIndexDescriptor[256];
		}
	}
}
