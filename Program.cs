using System;

namespace FFXivDatReader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string filePath = @"E:\Final Fantasy XIV\最终幻想XIV\game\My Games\FINAL FANTASY XIV - A Realm Reborn\FFXIV_CHR00438D8134C3D083\UISAVE.DAT";
            // const string filePath2 = @"E:\Final Fantasy XIV\最终幻想XIV\game\My Games\FINAL FANTASY XIV - A Realm Reborn\FFXIV_CHR00438D8134C3D083\ITEMFDR.DAT";
            new Config(filePath).printContent();
        }
    }
}