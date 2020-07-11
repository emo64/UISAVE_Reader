using System;
using System.Collections.Generic;
using System.IO;
using FFXivDatReader.ConfigSections;

namespace FFXivDatReader
{
    internal class Config
    {
        public Config(string fileName)
        {
            FileName = fileName;
            MagicNumber = 0x31;
            ReadFile();
        }

        protected string FileName { get; set; }

        protected byte MagicNumber { get; set; }
        protected byte[] Bytes0to3 { get; set; } = new byte[4];
        protected byte[] Bytes4to7 { get; set; } = new byte[4];
        protected uint NumValidBytes { get; set; }
        protected byte[] Bytes12to15 { get; set; } = new byte[4];
        protected byte[] Bytes16to23 { get; set; } = new byte[8];
        protected ulong ContentID { get; set; }

        public List<ConfigSection> Sections { get; } = new List<ConfigSection>();

        private void ReadFile()
        {
            //	Check that we can do stuff.
            if (!File.Exists(FileName))
            {
                throw new Exception("File does not exist (" + FileName + ")");
            }

            if (!BitConverter.IsLittleEndian)
            {
                throw new Exception("BitConverter is reporting Big-Endian, and we're not set up to deal with this.");
            }

            //	Read in the raw data.
            var rawData = File.ReadAllBytes(FileName);

            //	Parse the header.
            if (rawData.Length < 16)
            {
                throw new Exception(
                    "The file was not long enough to contain a full header.  Either the file or the header is corrupt, or the file was not completely read.");
            }

            Array.Copy(rawData, 0, Bytes0to3, 0, 4);
            Array.Copy(rawData, 4, Bytes4to7, 0, 4);
            NumValidBytes = BitConverter.ToUInt32(rawData, 8);
            Array.Copy(rawData, 12, Bytes12to15, 0, 4);

            //	Obtain the rest of the valid data and unpad it.
            if (rawData.Length < NumValidBytes + 16)
            {
                throw new Exception(
                    "The file was shorter than the header indicated.  Either the file or the header is corrupt, or the file was not completely read.");
            }

            var correctedData = xorBytes(rawData, 16, (int) NumValidBytes, MagicNumber);

            //	The first two parts after the header are not true sections, so handle them specifically.
            Array.Copy(rawData, 0, Bytes16to23, 0, 8);
            ContentID = BitConverter.ToUInt64(rawData, 8);

            //	Locals for processing sections.
            var currentOffset = 16u;

            //	And now we can start processing the sections.
            while (currentOffset < NumValidBytes)
            {
                uint sectionLength;
                ushort sectionId;
                if (NumValidBytes > currentOffset + 16u)
                {
                    //	Read Section Header
                    sectionId = BitConverter.ToUInt16(correctedData, (int) currentOffset);
                    //UNKNOWN: Six bytes of unknown use.
                    sectionLength = BitConverter.ToUInt32(correctedData, (int) currentOffset + 8);
                    //UNKNOWN: Another four bytes of unknown use.
                }
                else
                {
                    throw new Exception("Expected section header, but encountered premature end of valid file region.");
                }

                if (NumValidBytes > currentOffset + sectionLength)
                {
                    //	Create new section with data and add to list.
                    Sections.Add(CreateConfigSection(
                        sectionId,
                        new Span<byte>(correctedData, (int) currentOffset, 16).ToArray(),
                        new Span<byte>(correctedData, (int) currentOffset + 16, (int) sectionLength).ToArray(),
                        currentOffset));

                    //	Move our current offset to the end of the section, including the header and the four bytes of trailing padding.  Assuming the padding was probably reserved for an unimplemented checksum or something.
                    currentOffset += 16u + sectionLength + 4u;
                }
                else
                {
                    throw new Exception("Encountered premature end of valid file region while processing a section.");
                }
            }
        }

        private ConfigSection CreateConfigSection(ushort sectionId, byte[] sectionHeader, byte[] sectionData,
            uint fileOffsetBytes)
        {
            if (sectionId == 0x0)
            {
                return new ConfigSectionMailHistory(fileOffsetBytes, sectionHeader, sectionData);
            }

            if (sectionId == 0x4)
            {
                return new ConfigSectionSocial(fileOffsetBytes, sectionHeader, sectionData);
            }

            // else if( sectionId == 0x5 )
            // {
            // 	//	Teleport History
            // }
            // else if( sectionId == 0xD )
            // {
            // 	//	CWLS Info, contains at least CWLS ordering.
            // }
            // else 
            if (sectionId == 0x11)
            {
                return new ConfigSectionWaymarkPresets(fileOffsetBytes, sectionHeader, sectionData);
            }

            return new ConfigSectionUnknown(fileOffsetBytes, sectionHeader, sectionData);
        }

        private static byte[] xorBytes(byte[] source, int startIndex, int length, byte XORValue)
        {
            var newData = new Span<byte>(source, startIndex, length).ToArray();
            Array.Copy(source, startIndex, newData, 0, length);
            for (uint i = 0; i < newData.Length; ++i)
            {
                newData[i] ^= XORValue;
            }

            return newData;
        }

        public void printContent()
        {
            foreach (var section in Sections)
            {
                if (section is ConfigSectionUnknown)
                {
                    continue;
                }
                Console.WriteLine(section.ToString());
            }
        }
    }
}