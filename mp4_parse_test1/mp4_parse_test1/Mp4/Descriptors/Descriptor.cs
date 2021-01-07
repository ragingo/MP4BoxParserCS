namespace mp4_parse_test1.Descriptors
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

    // http://www.mp4ra.org/object.html
    public enum ObjectType : byte
    {
        Mpeg4Audio = 0x40
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
}
