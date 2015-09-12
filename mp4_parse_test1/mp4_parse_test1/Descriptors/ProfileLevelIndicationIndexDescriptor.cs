using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public class ProfileLevelIndicationIndexDescriptor : BaseDescriptor
	{
		public byte ProfileLevelIndicationIndex { get; set; }
		public ProfileLevelIndicationIndexDescriptor()
		{
			Tag = DescriptorTag.ProfileLevelIndicationIndexDescrTag;
		}
	}
}
