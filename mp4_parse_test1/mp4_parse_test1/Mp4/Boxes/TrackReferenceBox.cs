using System;

namespace mp4_parse_test1.Boxes
{
    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.3.3 Track Reference Box
    /// </summary>
    public class TrackReferenceBox : Box
    {
        public TrackReferenceBox() : base(BoxType.tref)
        {
        }
    }

    /// <summary>
    /// ISO/IEC 14496-12:2012(E) 8.3.3 Track Reference Box
    /// </summary>
    public class TrackReferenceTypeBox : Box
    {
        public UInt32[] TrackIDs { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="referenceType">'hint' or 'cdsc' or 'hind' or 'vdep' or 'vplx'</param>
        public TrackReferenceTypeBox(BoxType referenceType) : base(referenceType)
        {
        }
    }
}
