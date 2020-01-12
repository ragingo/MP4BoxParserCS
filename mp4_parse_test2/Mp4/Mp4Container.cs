using System;
using System.Buffers;
using mp4_parse_test2.Mp4.Types;

namespace mp4_parse_test2.Mp4
{
    class Mp4Container
    {
        private long _containerOffset;
        private long _bufferOffset;
        private long _totalReceivedLength;
        private Box _currentBox;

        public void ParsePartialData(ReadOnlySequence<byte> seq)
        {
            _totalReceivedLength += seq.Length;

            // 次のボックスの先頭まで受信ができてなかったら抜ける
            if (_containerOffset > _totalReceivedLength)
            {
                return;
            }

            long startOffset = seq.Length - (_totalReceivedLength - _containerOffset);

            _bufferOffset = startOffset;

            while (_bufferOffset < seq.Length)
            {
                var span = seq.Slice(_bufferOffset).FirstSpan;

                _currentBox = new Box(span);
                _bufferOffset += _currentBox.Size;
                Console.WriteLine(_currentBox);

                switch (_currentBox.Type)
                {
                    case BoxType.ftyp:
                        {
                            //var ftyp = new FileTypeBox(ref _currentBox, span.Slice(_offset));
                        }
                        break;

                    default:
                        break;
                }
            }

            _containerOffset += _bufferOffset;
        }
    }
}
