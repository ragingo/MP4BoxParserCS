namespace mp4_parse_test1.Descriptors
{
    // ISO_IEC_14496-3 1.6.2.1.
    // TODO: 実装中
    public class AudioSpecificConfig : DecoderSpecificInfo
    {
        public AudioObjectType AudioObjectType { get; set; } // 5 bits uimsbf
        public byte SamplingFrequencyIndex { get; set; } // 4 bits bslbf

        public AudioSpecificConfig()
        {
        }
    }

    // https://en.wikipedia.org/wiki/MPEG-4_Part_3#MPEG-4_Audio_Object_Types
    public enum AudioObjectType
    {
        AAC_Main = 1,
        AAC_LC_Low_Complexity = 2,
        AAC_SSR_Scalable_Sample_Rate = 3,
        AAC_LTP_Long_Term_Prediction = 4,
        SBR_Spectral_Band_Replication = 5,
        AAC_Scalable = 6,
        TwinVQ = 7,
        CELP_Code_Excited_Linear_Prediction = 8,
        HVXC_Harmonic_Vector_eXcitation_Coding = 9,
        Reserved1 = 10,
        Reserved2 = 11,
        TTSI_Text_To_Speech_Interface = 12,
        Main_synthesis = 13,
        wavetable_sample_based_synthesis = 14,
        General_MIDI = 15,
        Algorithmic_Synthesis_and_AudioEffects = 16,
        ER_AAC_LC = 17,
        Reserved3 = 18,
        ER_AAC_LTP = 19,
        ER_AAC_Scalable = 20,
        ER_TwinVQ = 21,
        ER_BSAC_Bit_Sliced_Arithmetic_Coding = 22,
        ER_AAC_LD_Low_Delay = 23,
        ER_CELP = 24,
        ER_HVXC = 25,
        ER_HILN_Harmonic_and_Individual_Lines_plus_Noise = 26,
        ER_Parametric = 27,
        SSC_SinuSoidal_Coding = 28,
        PS_Parametric_Stereo = 29,
        MPEG_Surround = 30,
        Reserved4 = 31,
        MPEG_1_2_Layer_1 = 32,
        MPEG_1_2_Layer_2 = 33,
        MPEG_1_2_Layer_3 = 34,
        DST_Direct_Stream_Transfer = 35,
        ALS_Audio_Lossless_Coding = 36,
        SLS_Scalable_Lossless_Coding = 37,
        SLS_non_core = 38,
        ER_AAC_ELD_Enhanced_Low_Delay = 39,
        SMR_Symbolic_Music_Representation_Simple = 40,
        SMR_Main = 41,
        USAC_Unified_Speech_and_AudioCoding_no_SBR = 42,
        SAOC_Spatial_Audio_Object_Coding = 43,
        LD_MPEG_Surround = 44,
        USAC = 45,
    }
}
