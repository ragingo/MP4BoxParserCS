﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public /*abstract*/ class DecoderSpecificInfo : BaseDescriptor
	{
		public DecoderSpecificInfo()
		{
			Tag = DescriptorTag.DecSpecificInfoTag;
		}
	}
}