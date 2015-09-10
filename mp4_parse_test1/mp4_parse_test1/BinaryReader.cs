using System;
using System.IO;
using System.Text;

namespace mp4_parse_test1
{
	class BinaryReader : System.IO.BinaryReader
	{
		private bool _isBigEndian = false;
		private readonly bool _isNeedReverse;

		public BinaryReader(Stream input, bool isBigEndian = false)
			: base(input)
		{
			_isBigEndian = isBigEndian;

			if (BitConverter.IsLittleEndian && _isBigEndian ||
				!BitConverter.IsLittleEndian && !_isBigEndian)
			{
				_isNeedReverse = true;
			}
			else
			{
				_isNeedReverse = false;
			}
		}
		public BinaryReader(Stream input, Encoding encoding, bool isBigEndian = false)
			: base(input, encoding)
		{
			_isBigEndian = isBigEndian;
		}
		public BinaryReader(Stream input, Encoding encoding, bool leaveOpen, bool isBigEndian = false)
			: base(input, encoding, leaveOpen)
		{
			_isBigEndian = isBigEndian;
		}
		public override byte[] ReadBytes(int count)
		{
			byte[] bytes = base.ReadBytes(count);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
		public override char[] ReadChars(int count)
		{
			char[] chars = base.ReadChars(count);
			if (_isNeedReverse)
			{
				Array.Reverse(chars);
			}
			return chars;
		}
		public override decimal ReadDecimal()
		{
			if (_isNeedReverse)
			{
				throw new NotSupportedException();
			}
			return base.ReadDecimal();
		}
		public override double ReadDouble()
		{
			byte[] bytes = base.ReadBytes(8);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToDouble(bytes, 0);
		}
		public override short ReadInt16()
		{
			byte[] bytes = base.ReadBytes(2);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt16(bytes, 0);
		}
		public override int ReadInt32()
		{
			byte[] bytes = base.ReadBytes(4);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}
		public override long ReadInt64()
		{
			byte[] bytes = base.ReadBytes(8);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt64(bytes, 0);
		}
		public override float ReadSingle()
		{
			byte[] bytes = base.ReadBytes(4);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToSingle(bytes, 0);
		}
		public override ushort ReadUInt16()
		{
			byte[] bytes = base.ReadBytes(2);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToUInt16(bytes, 0);
		}
		public override UInt32 ReadUInt32()
		{
			byte[] bytes = base.ReadBytes(4);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToUInt32(bytes, 0);
		}
		public override ulong ReadUInt64()
		{
			byte[] bytes = base.ReadBytes(8);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToUInt64(bytes, 0);
		}
		public UInt32 ReadUInt24()
		{
			byte[] bytes = new byte[4];
			Array.Copy(base.ReadBytes(3), 0, bytes, 0, 3);
			if (_isNeedReverse)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToUInt32(bytes, 0);
		}

	}
}
