using System;

namespace FFXivDatReader
{
    public abstract class ConfigSection
    {
        protected ConfigSection(uint fileOffsetBytes, byte[] sectionHeader, byte[] sectionData)
        {
            FileOffset = fileOffsetBytes;

            if (sectionHeader.Length != 16)
                throw new Exception("Section header was of an unexpected size.  File Offset: 0x" +
                                    fileOffsetBytes.ToString("X"));
            SectionHeader = new Span<byte>(sectionHeader).ToArray();
            SectionID = BitConverter.ToUInt16(SectionHeader, 0);
            SectionLength_Bytes = BitConverter.ToUInt32(SectionHeader, 8);

            if (sectionData.Length != SectionLength_Bytes)
                throw new Exception("Section data was of an unexpected size.  File Offset: 0x" +
                                    fileOffsetBytes.ToString("X"));
            SectionData = new Span<byte>(sectionData).ToArray();

            Read();
        }

        public uint FileOffset { get; protected set; }
        public byte[] SectionHeader { get; protected set; }
        public ushort SectionID { get; protected set; }
        public uint SectionLength_Bytes { get; protected set; }
        public byte[] SectionData { get; protected set; }

        protected abstract void Read();
        public abstract override string ToString();
    }
}