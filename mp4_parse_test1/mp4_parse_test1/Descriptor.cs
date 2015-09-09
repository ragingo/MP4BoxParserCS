using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	// ISO/IEC 14496-1:2010(E) 7.2.2.1 Table 1 — List of Class Tags for Descriptors
	public enum DescriptorTag : byte
	{
		Forbidden = 0x00,
		ObjectDescrTag = 0x01,
		InitialObjectDescrTag = 0x02,
		ES_DescrTag = 0x03,
		DecoderConfigDescrTag = 0x04,
		DecSpecificInfoTag = 0x05,
		SLConfigDescrTag = 0x06,
		ContentIdentDescrTag = 0x07,
		SupplContentIdentDescrTag = 0x08,
		IPI_DescrPointerTag = 0x09,
		IPMP_DescrPointerTag = 0x0A,
		IPMP_DescrTag = 0x0B,
		QoS_DescrTag = 0x0C,
		RegistrationDescrTag = 0x0D,
		ES_ID_IncTag = 0x0E,
		ES_ID_RefTag = 0x0F,
		MP4_IOD_Tag = 0x10,
		MP4_OD_Tag = 0x11,
		IPL_DescrPointerRefTag = 0x12,
		ExtensionProfileLevelDescrTag = 0x13,
		ProfileLevelIndicationIndexDescrTag = 0x14,
		ReservedForIsoUsed_Min = 0x15,
		ReservedForIsoUsed_Max = 0x3F,
		ContentClassificationDescrTag = 0x40,
		KeyWordDescrTag = 0x41,
		RatingDescrTag = 0x42,
		LanguageDescrTag = 0x43,
		ShortTextualDescrTag = 0x44,
		ExpandedTextualDescrTag = 0x45,
		ContentCreatorNameDescrTag = 0x46,
		ContentCreationDateDescrTag = 0x47,
		OCICreatorNameDescrTag = 0x48,
		OCICreationDateDescrTag = 0x49,
		SmpteCameraPositionDescrTag = 0x4A,
		SegmentDescrTag = 0x4B,
		MediaTimeDescrTag = 0x4C,
		ReservedForIsoUsed_OciExtensions_Min = 0x4D,
		ReservedForIsoUsed_OciExtensions_Max = 0x5F,
		IPMP_ToolsListDescrTag = 0x60,
		IPMP_ToolTag = 0x61,
		M4MuxTimingDescrTag = 0x62,
		M4MuxCodeTableDescrTag = 0x63,
		ExtSLConfigDescrTag = 0x64,
		M4MuxBufferSizeDescrTag = 0x65,
		M4MuxIdentDescrTag = 0x66,
		DependencyPointerTag = 0x67,
		DependencyMarkerTag = 0x68,
		M4MuxChannelDescrTag = 0x69,
		ReservedForIsoUsed2_Min = 0x6A,
		ReservedForIsoUsed2_Max = 0xBF,
		UserPrivate_Min = 0xC0,
		UserPrivate_Max = 0xFE,
		Forbidden2 = 0xFF,
	}

	public abstract class BaseDescriptor
	{
		public DescriptorTag Tag { get; set; }
		public BaseDescriptor()
		{
		}
	}

	public abstract class IP_IdentificationDataSet : BaseDescriptor
	{
		public IP_IdentificationDataSet()
		{
		}
	}

	public abstract class OCI_Descriptor : BaseDescriptor
	{
		public OCI_Descriptor()
		{
		}
	}

	public abstract class DecoderSpecificInfo : BaseDescriptor
	{
		public DecoderSpecificInfo()
		{
			Tag = DescriptorTag.DecSpecificInfoTag;
		}
	}

	public class IPI_DescrPointer : BaseDescriptor
	{
		public UInt16 IPI_ES_Id { get; set; }
		public IPI_DescrPointer()
		{
			Tag = DescriptorTag.IPI_DescrPointerTag;
		}
	}

	public class LanguageDescriptor : OCI_Descriptor
	{
		public UInt32 LanguageCode { get; set; } // 24 bits
		public LanguageDescriptor()
		{
			Tag = DescriptorTag.LanguageDescrTag;
		}
	}

	public class ProfileLevelIndicationIndexDescriptor : BaseDescriptor
	{
		public byte ProfileLevelIndicationIndex { get; set; }
		public ProfileLevelIndicationIndexDescriptor()
		{
			Tag = DescriptorTag.ProfileLevelIndicationIndexDescrTag;
		}
	}

	// http://www.mp4ra.org/object.html
	public enum ObjectType : byte
	{
		Mpeg4Audio = 0x40
	}

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

	public class SLConfigDescriptor : BaseDescriptor
	{
		// TODO: members...めんどくさすぎる

		public SLConfigDescriptor()
		{
			Tag = DescriptorTag.SLConfigDescrTag;
		}
	}

	public class IPMP_DescriptorPointer : BaseDescriptor
	{
		public byte IPMP_DescriptorID { get; set; }
		public UInt16 IPMP_DescriptorIDEx { get; set; } // IPMP_DescriptorID == 0xff の場合のみ存在する
		public UInt16 IPMP_ES_ID { get; set; } // IPMP_DescriptorID == 0xff の場合のみ存在する

		public IPMP_DescriptorPointer()
		{
			Tag = DescriptorTag.IPMP_DescrTag;
		}
	}

	public class QoS_Descriptor : BaseDescriptor
	{
		public byte PreDefined { get; set; }
		public QoS_Qualifier[] qualifiers;
		public QoS_Descriptor()
		{
			Tag = DescriptorTag.QoS_DescrTag;
			qualifiers = new QoS_Qualifier[8];
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

	public /*abstract*/ class ExtensionDescriptor : BaseDescriptor
	{
	}

	// ISO/IEC 14496-1:2010(E) 7.2.6.4.2 Table 3 — ODProfileLevelIndication Values
	public enum ODProfileLevelIndication : byte
	{
		Forbidden = 0x00,
		ReservedForIsoUse_NoSLExtension = 0x01,
		ReservedForIsoUse_SLExtension_Min = 0x02,
		ReservedForIsoUse_SLExtension_Max = 0x7F,
		ReservedForIsoUse_Min = 0x03,
		ReservedForIsoUse_Max = 0x7F,
		UserPrivate_Min = 0x80,
		UserPrivate_Max = 0xFD,
		NoODProfileSpecified = 0xFE,
		NoODCapabilityRequired = 0xFF,
	}

	public class ExtensionProfileLevelDescriptor : ExtensionDescriptor
	{
		byte ProfileLevelIndicationIndex { get; set; }
		ODProfileLevelIndication ODProfileLevelIndication { get; set; }
		byte SceneProfileLevelIndication { get; set; }
		byte AudioProfileLevelIndication { get; set; }
		byte VisualProfileLevelIndication { get; set; }
		byte GraphicsProfileLevelIndication { get; set; }
		byte MPEGJProfileLevelIndication { get; set; }
		byte TextProfileLevelIndication { get; set; }
		byte ThreeDCProfileLevelIndication { get; set; }

		public ExtensionProfileLevelDescriptor()
		{
			Tag = DescriptorTag.ExtensionProfileLevelDescrTag;
		}
	}

	public class ESDescriptor : BaseDescriptor
	{
		public UInt16 ESID { get; set; }
		public byte StreamDependenceFlag { get; set; } // 1 bit
		public byte UrlFlag { get; set; } // 1 bit
		public byte OcrStreamFlag { get; set; } // 1 bit
		public byte StreamPriority { get; set; } // 5 bits
		public UInt16 DependsOnESID { get; set; } // StreamDependenceFlag が 0以外 の場合のみ存在する
		public byte UrlLength { get; set; } // URL_Flag が 0以外 の場合のみ存在する
		public string UrlString { get; set; } // URL_Flag が 0以外 の場合のみ存在する
		public UInt16 OcrESID { get; set; } // OCRstreamFlag  が 0以外 の場合のみ存在する
		public DecoderConfigDescriptor DecConfigDescr { get; set; }
		public SLConfigDescriptor SlConfigDescr { get; set; }
		public IPI_DescrPointer[] IpiPtr { get; set; }
		public IP_IdentificationDataSet[] IpIDS { get; set; }
		public IPMP_DescriptorPointer[] IpmpDescrPtr { get; set; }
		public LanguageDescriptor[] LangDescr { get; set; }
		public QoS_Descriptor[] QosDescr { get; set; }
		public RegistrationDescriptor[] RegDescr { get; set; }
		public ExtensionDescriptor[] ExtDescr { get; set; }

		public ESDescriptor()
		{
			Tag = DescriptorTag.ES_DescrTag;
			DecConfigDescr = new DecoderConfigDescriptor();
			SlConfigDescr = new SLConfigDescriptor();
			IpiPtr = new IPI_DescrPointer[2];
			IpIDS = new IP_IdentificationDataSet[256];
			IpmpDescrPtr = new IPMP_DescriptorPointer[256];
			LangDescr = new LanguageDescriptor[256];
			QosDescr = new QoS_Descriptor[2];
			RegDescr = new RegistrationDescriptor[2];
			ExtDescr = new ExtensionDescriptor[256];
		}
	}
}
