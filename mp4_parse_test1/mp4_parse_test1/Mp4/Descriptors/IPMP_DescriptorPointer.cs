using System;

namespace mp4_parse_test1.Descriptors
{
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
}
