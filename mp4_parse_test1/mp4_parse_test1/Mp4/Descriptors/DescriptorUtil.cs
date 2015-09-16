using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1.Descriptors
{
	public static class DescriptorUtil
	{
		private static readonly Dictionary<ObjectType, Type> ClassMapping =
			new Dictionary<ObjectType, Type> {
				{ ObjectType.Mpeg4Audio, typeof(AudioSpecificConfig) },
			};
		private static readonly KeyValuePair<ObjectType, Type> DefaultValue = new KeyValuePair<ObjectType, Type>();

		public static DecoderSpecificInfo CreateInstance(ObjectType type)
		{
			var pair = ClassMapping.FirstOrDefault(x => x.Key == type);

			if (DefaultValue.Equals(pair))
			{
				return new DecoderSpecificInfo();
			}

			var instance = Activator.CreateInstance(pair.Value) as DecoderSpecificInfo;

			return instance;
		}

	}
}
