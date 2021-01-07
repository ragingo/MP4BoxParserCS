namespace mp4_parse_test1.Aac
{
    public class AdtsHeaderParser
    {
        private byte[] _bytes;

        public AdtsHeaderParser(byte[] bytes)
        {
            _bytes = bytes;
        }

        public AdtsHeader Parse()
        {
            var adts = new AdtsHeader();
            return adts;
        }

        public static byte[] ToBytes(AdtsHeader header)
        {
            byte[] bytes = new byte[7];

            //bytes[0] = header.SyncWord & 

            return bytes;
        }
    }
}
