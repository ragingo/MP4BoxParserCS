using System;
using System.Collections.Generic;

namespace mp4_parse_test1.Boxes
{
    public interface IDataEntryBox
    {
        BoxType Type { get; }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.7.2 Data Reference Box
    /// DataReferenceBox
    /// </summary>
    public class DataReferenceBox : FullBox
    {
        public UInt32 EntryCount { get; set; }
        public List<IDataEntryBox> Entries;

        public DataReferenceBox() : base(BoxType.dref)
        {
            Entries = new List<IDataEntryBox>();
        }
    }
}
