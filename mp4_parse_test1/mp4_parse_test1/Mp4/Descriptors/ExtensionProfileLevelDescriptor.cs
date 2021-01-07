namespace mp4_parse_test1.Descriptors
{
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
}
