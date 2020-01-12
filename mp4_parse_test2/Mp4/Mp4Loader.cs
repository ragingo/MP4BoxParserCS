using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using mp4_parse_test2.Mp4.Types;

namespace mp4_parse_test2.Mp4
{
    // TODO: Mp4Reader, MoveNextBox() で能動的に取りにいけるように変えたい
    class Mp4Loader : IDisposable
    {
        private Stream _stream;
        private readonly Mp4Container _mp4Container = new Mp4Container();

        public Uri Uri { get; private set; }

        public event Action<Box> BoxLoaded
        {
            add { _mp4Container.BoxLoaded += value; }
            remove { _mp4Container.BoxLoaded -= value; }
        }

        public Mp4Loader(Uri uri)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public async Task LoadAsync()
        {
            if (Uri.IsFile)
            {
                _stream = LoadFromFile();
            }
            else if (Uri.Scheme == "http" || Uri.Scheme == "https")
            {
                // TODO: 
            }

            _stream ??= Stream.Null;

            await ReadStreamAsync().ConfigureAwait(false);
        }

        private Stream LoadFromFile()
        {
            if (!File.Exists(Uri.AbsolutePath))
            {
                return null;
            }

            return new FileStream(Uri.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private async Task ReadStreamAsync()
        {
            if (!_stream.CanRead)
            {
                return;
            }

            if (_stream.Length == 0)
            {
                return;
            }

            var pipe = new Pipe();
            var writer = WritePipeAsync(pipe.Writer, _stream);
            var reader = ReadPipeAsync(pipe.Reader);
            await Task.WhenAll(writer, reader).ConfigureAwait(false);
        }

        private static async Task WritePipeAsync(PipeWriter writer, Stream stream)
        {
            while (true)
            {
                var memory = writer.GetMemory();
                int len = await stream.ReadAsync(memory);
                if (len == 0)
                {
                    break;
                }

                writer.Advance(len);

                var result = await writer.FlushAsync();
                if (result.IsCompleted)
                {
                    break;
                }
            }

            await writer.CompleteAsync();
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                if (result.IsCompleted)
                {
                    break;
                }

                var buffer = result.Buffer;
                if (!buffer.IsEmpty)
                {
                    _mp4Container.ParsePartialData(buffer);
                }

                reader.AdvanceTo(result.Buffer.End);
            }

            await reader.CompleteAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    _stream.Dispose();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~Mp4Loader()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
