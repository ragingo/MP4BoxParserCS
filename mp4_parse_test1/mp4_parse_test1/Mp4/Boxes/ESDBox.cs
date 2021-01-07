using mp4_parse_test1.Descriptors;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-14:2003(E) 5.6 Sample Description Boxes
    /// ESDBox
    /// </summary>
    public class EsdBox : FullBox
    {
        public ESDescriptor ES { get; set; }

        public EsdBox() : base(BoxType.esds)
        {
            ES = new ESDescriptor();
        }
    }

    /// <summary>
    /// ISO/IEC 14496-14:2003(E) 5.6 Sample Description Boxes
    /// MP4VisualSampleEntry
    /// </summary>
    public class Mp4VisualSampleEntry : VisualSampleEntry
    {
        public Mp4VisualSampleEntry() : base(SampleEntryCode.mp4v)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-14:2003(E) 5.6 Sample Description Boxes
    /// MP4AudioSampleEntry
    /// </summary>
    public class Mp4AudioSampleEntry : AudioSampleEntry
    {
        public Mp4AudioSampleEntry() : base(SampleEntryCode.mp4a)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-14:2003(E) 5.6 Sample Description Boxes
    /// MpegSampleEntry
    /// </summary>
    public class MpegSampleEntry : SampleEntry
    {
        public MpegSampleEntry() : base(SampleEntryCode.mp4s)
        {
        }
    }
}
