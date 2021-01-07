using System;

namespace mp4_parse_test1.Descriptors
{
    public class LanguageDescriptor : OCI_Descriptor
    {
        public UInt32 LanguageCode { get; set; } // 24 bits
        public LanguageDescriptor()
        {
            Tag = DescriptorTag.LanguageDescrTag;
        }
    }
}
