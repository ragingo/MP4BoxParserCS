using System;

namespace mp4_parse_test1.Descriptors
{
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
