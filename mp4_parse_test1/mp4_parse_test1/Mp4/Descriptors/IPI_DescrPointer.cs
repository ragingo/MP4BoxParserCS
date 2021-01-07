using System;

namespace mp4_parse_test1.Descriptors
{
    public class IPI_DescrPointer : BaseDescriptor
    {
        public UInt16 IPI_ES_Id { get; set; }
        public IPI_DescrPointer()
        {
            Tag = DescriptorTag.IPI_DescrPointerTag;
        }
    }
}
