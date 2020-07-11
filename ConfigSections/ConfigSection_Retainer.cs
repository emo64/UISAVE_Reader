using System;

namespace FFXivDatReader.ConfigSections
{
    internal class ConfigSectionRetainer : ConfigSection
    {
        public ConfigSectionRetainer(uint fileOffsetBytes, byte[] sectionHeader, byte[] sectionData) :
            base(fileOffsetBytes, sectionHeader, sectionData)
        {
        }

        protected override void Read()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            // var result = BitConverter.ToString(SectionData).Replace("-", " ");
            // return result;
            return "";
        }
    }
}