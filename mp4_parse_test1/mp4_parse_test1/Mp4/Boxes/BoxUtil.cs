using System;
using System.Collections.Generic;
using System.Linq;

namespace mp4_parse_test1.Boxes
{
	public static class BoxUtil
	{
		private static readonly Dictionary<BoxType, Type> ClassMapping =
			new Dictionary<BoxType, Type> {
				{ BoxType.Unknown, typeof(Box) },
				{ BoxType.ftyp, typeof(FileTypeBox) },
				{ BoxType.mdat, typeof(MediaDataBox) },
				{ BoxType.moov, typeof(MovieBox) },
				{ BoxType.mvhd, typeof(MovieHeaderBox) },
				{ BoxType.mdia, typeof(MediaBox) },
				{ BoxType.hdlr, typeof(HandlerBox) },
				{ BoxType.minf, typeof(MediaInformationBox) },
				{ BoxType.stbl, typeof(SampleTableBox) },
				{ BoxType.stsd, typeof(SampleDescriptionBox) },
				{ BoxType.stts, typeof(TimeToSampleBox) },
				{ BoxType.stsc, typeof(SampleToChunkBox) },
				{ BoxType.stsz, typeof(SampleSizeBox) },
				{ BoxType.stco, typeof(ChunkOffsetBox) },
				{ BoxType.esds, typeof(EsdBox) },
				{ BoxType.avc1, typeof(Mp4VisualSampleEntry) }, // TODO: とりあえず使っておく
				{ BoxType.mp4v, typeof(Mp4VisualSampleEntry) },
				{ BoxType.mp4a, typeof(Mp4AudioSampleEntry) },
			};
		private static readonly KeyValuePair<BoxType, Type> DefaultValue = new KeyValuePair<BoxType, Type>();

		public static Box CreateInstance(BoxType type)
		{
			var pair = ClassMapping.FirstOrDefault(x => x.Key == type);

			if (DefaultValue.Equals(pair))
			{
				return new Box(type);
			}

			return Activator.CreateInstance(pair.Value) as Box;
		}
	}
}
