using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public class RegistrationDescriptor : BaseDescriptor
	{
		public UInt32 FormatIdentifier { get; set; }
		public byte[] AdditionalIdentificationInfo { get; set; } // TODO: 幾つ確保すればいいのか分からない。 [sizeOfInstance-4]
		public RegistrationDescriptor()
		{
			Tag = DescriptorTag.RegistrationDescrTag;
			AdditionalIdentificationInfo = new byte[256]; // TODO: とりあえず 256
		}
	}

}
