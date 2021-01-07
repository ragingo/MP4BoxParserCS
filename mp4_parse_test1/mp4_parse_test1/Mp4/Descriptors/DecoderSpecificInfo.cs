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
