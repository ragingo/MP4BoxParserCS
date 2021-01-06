using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public class QoS_Descriptor : BaseDescriptor
	{
		public byte PreDefined { get; set; }
		public QoS_Qualifier[] Qualifiers;
		public QoS_Descriptor()
		{
			Tag = DescriptorTag.QoS_DescrTag;
			Qualifiers = new QoS_Qualifier[8];
		}
	}

	public abstract class QoS_Qualifier
	{
		public byte Tag { get; set; }
	}

	public class QoS_Qualifier_MAX_DELAY : QoS_Qualifier
	{
		public UInt32 MAX_DELAY { get; set; }
		public QoS_Qualifier_MAX_DELAY()
		{
			Tag = 0x01;
		}
	}

	public class QoS_Qualifier_PREF_MAX_DELAY : QoS_Qualifier
	{
		public UInt32 PREF_MAX_DELAY { get; set; }
		public QoS_Qualifier_PREF_MAX_DELAY()
		{
			Tag = 0x02;
		}
	}

	public class QoS_Qualifier_LOSS_PROB : QoS_Qualifier
	{
		public UInt32 LOSS_PROB { get; set; }
		public QoS_Qualifier_LOSS_PROB()
		{
			Tag = 0x03;
		}
	}

	public class QoS_Qualifier_MAX_GAP_LOSS : QoS_Qualifier
	{
		public UInt32 MAX_GAP_LOSS { get; set; }
		public QoS_Qualifier_MAX_GAP_LOSS()
		{
			Tag = 0x04;
		}
	}

	public class QoS_Qualifier_MAX_AU_SIZE : QoS_Qualifier
	{
		public UInt32 MAX_AU_SIZE { get; set; }
		public QoS_Qualifier_MAX_AU_SIZE()
		{
			Tag = 0x41;
		}
	}

	public class QoS_Qualifier_AVG_AU_SIZE : QoS_Qualifier
	{
		public UInt32 AVG_AU_SIZE { get; set; }
		public QoS_Qualifier_AVG_AU_SIZE()
		{
			Tag = 0x42;
		}
	}

	public class QoS_Qualifier_MAX_AU_RATE : QoS_Qualifier
	{
		public UInt32 MAX_AU_RATE { get; set; }
		public QoS_Qualifier_MAX_AU_RATE()
		{
			Tag = 0x43;
		}
	}

	public class QoS_Qualifier_REBUFFERING_RATIO : QoS_Qualifier
	{
		public UInt32 REBUFFERING_RATIO { get; set; }
		public QoS_Qualifier_REBUFFERING_RATIO()
		{
			Tag = 0x44;
		}
	}
}
