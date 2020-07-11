using System;
using System.Text;

namespace FFXivDatReader.ConfigSections
{
    internal class ConfigSectionUnknown : ConfigSection
    {
        public ConfigSectionUnknown(uint fileOffsetBytes, byte[] sectionHeader, byte[] sectionData) :
            base(fileOffsetBytes, sectionHeader, sectionData)
        {
        }

        protected override void Read()
        {
            
        }

        public override string ToString()
        {
            return "Unknown Section Type (ID: " + SectionID + ", Offset in File (includes section header): 0x" +
                   FileOffset.ToString("X") + ", Length: 0x" + SectionData.Length.ToString("X") +
                   " bytes (excluding section header))";
            // return "Unknown Section Type (ID: " + SectionID + ", Offset in File (includes section header): 0x" +
            //        FileOffset.ToString("X") + ", Length: 0x" + SectionData.Length.ToString("X") +
            //        " bytes (excluding section header))"; + $"\n{Encoding.UTF8.GetString(SectionData).Replace(" ", "")}";
        }
    }
}