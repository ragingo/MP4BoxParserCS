﻿using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// SampleEntry
    /// </summary>
    public class SampleEntry : Box
    {
        public byte[] Reserved { get; private set; }
        public UInt16 DataReferenceIndex { get; set; }

        public SampleEntry(BoxType type) : base(type)
        {
            Reserved = new byte[6];
        }

        public SampleEntry(SampleEntryCode code) : this((BoxType)code)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// HintSampleEntry
    /// </summary>
    public class HintSampleEntry : SampleEntry
    {
        public List<byte> Data { get; private set; }

        public HintSampleEntry(BoxType type) : base(type)
        {
            Data = new List<byte>();
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// MetaDataSampleEntry
    /// </summary>
    public class MetaDataSampleEntry : SampleEntry
    {
        public MetaDataSampleEntry(BoxType type) : base(type)
        {
        }
        public MetaDataSampleEntry(SampleEntryCode code) : base(code)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// XMLMetaDataSampleEntry
    /// </summary>
    public class XMLMetaDataSampleEntry : MetaDataSampleEntry
    {
        public string ContentEncoding { get; set; }
        public string Namespace { get; set; }
        public string Schemalocation { get; set; }
        public BitRateBox BitRateBox { get; set; }

        public XMLMetaDataSampleEntry() : base(SampleEntryCode.metx)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// TextMetaDataSampleEntry
    /// </summary>
    public class TextMetaDataSampleEntry : MetaDataSampleEntry
    {
        public string ContentEncoding { get; set; }
        public string MimeFormat { get; set; }
        public BitRateBox BitRateBox { get; set; }

        public TextMetaDataSampleEntry() : base(SampleEntryCode.mett)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// URIMetaSampleEntry
    /// </summary>
    public class URIMetaSampleEntry : MetaDataSampleEntry
    {
        public URIBox TheLabel { get; set; }
        public URIInitBox Init { get; set; }

        public URIMetaSampleEntry() : base(BoxType.urim)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// VisualSampleEntry
    /// </summary>
    public class VisualSampleEntry : SampleEntry
    {
        public UInt16 PreDefined { get; set; }
        public UInt16 Reserved2 { get; set; }
        public UInt32[] PreDefined2 { get; set; }
        public UInt16 Width { get; set; }
        public UInt16 Height { get; set; }
        public UInt32 HorizontalResolution { get; set; }
        public UInt32 VerticalResolution { get; set; }
        public UInt32 Reserved3 { get; set; }
        public UInt16 FrameCount { get; set; }
        public string CompressorName { get; set; }
        public UInt16 Depth { get; set; }
        public Int32 PreDefined3 { get; set; }
        public CleanApertureBox Clap { get; set; }
        public PixelAspectRatioBox Pasp { get; set; }

        public VisualSampleEntry(SampleEntryCode code) : base(code)
        {
            PreDefined2 = new UInt32[3];
            HorizontalResolution = 0x00480000 >> 16;
            VerticalResolution = 0x00480000 >> 16;
            FrameCount = 1;
            Depth = 0x0018 >> 8;
            PreDefined3 = -1;
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.5.2 Sample Description Box
    /// AudioSampleEntry
    /// </summary>
    public class AudioSampleEntry : SampleEntry
    {
        public new UInt32[] Reserved { get; set; }
        public UInt16 ChannelCount { get; set; }
        public UInt16 SampleSize { get; set; }
        public UInt16 PreDefined { get; set; }
        public UInt16 Reserved2 { get; set; }
        public UInt32 SampleRate { get; set; }

        public AudioSampleEntry(SampleEntryCode code) : base(code)
        {
            Reserved = new UInt32[2];
            ChannelCount = 2;
            SampleSize = 16;
        }
    }
}
