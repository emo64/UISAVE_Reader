using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFXivDatReader.Ext;
using Microsoft.VisualBasic;

namespace FFXivDatReader.ConfigSections
{
    public class ContactEntry
    {
        public string playerName { get; set; }
        public ClassJob classJob { get; set; }
        public WorldServer homeWorldServer { get; set; }
    }
    
    public class FriendListEntry
    {
        public string playerName { get; set; }
        public WorldServer homeWorldServer { get; set; }

        protected bool Equals(FriendListEntry other)
        {
            return playerName == other.playerName && homeWorldServer == other.homeWorldServer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((FriendListEntry) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(playerName, (int) homeWorldServer);
        }
    }
    
    public class ConfigSectionSocial : ConfigSection
    {
        private HashSet<ContactEntry> _recentContactList = new HashSet<ContactEntry>();
        private HashSet<FriendListEntry> _friendList = new HashSet<FriendListEntry>();
        
        public ConfigSectionSocial(uint fileOffsetBytes, byte[] sectionHeader, byte[] sectionData) :
            base(fileOffsetBytes, sectionHeader, sectionData)
        {
            
        }

        protected override void Read()
        {
            // Contains Friends Group Titles, Recent Contacts List, Blacklist, and Some friends list-related things.  Possibly a bit more.
            var offset = 624;
            for (var index = 0; index < 50; index++)
            {
                var readOnlySpan = new ReadOnlySpan<byte>(SectionData, offset + 4, 32);
                if (readOnlySpan.ToArray().All(singleByte => singleByte == 0)) break;
                var playerName = Encoding.UTF8.GetString(readOnlySpan);
                var serverId = (WorldServer) BitConverter.ToUInt16(SectionData, offset + 44);
                var classJob = (ClassJob) BitConverter.ToUInt16(SectionData, offset + 46);
                // Console.WriteLine($"{index.ToString().PadLeft(2)} > {classJob.ToString().PadRight(15)} {playerName}@{serverId}");
                _recentContactList.Add(new ContactEntry
                {
                    playerName = playerName,
                    classJob = classJob,
                    homeWorldServer = serverId
                });
                offset += 48;
            }
            offset = 11896;
            for (var index = 0; index < 200; index++)
            {
                var readOnlySpan = new ReadOnlySpan<byte>(SectionData, offset, 32);
                if (readOnlySpan.ToArray().All(singleByte => singleByte == 0)) break;
                var playerName = Encoding.UTF8.GetString(readOnlySpan);
                var serverId = (WorldServer) BitConverter.ToUInt16(SectionData, offset + 40);
                // Console.WriteLine($"{index.ToString().PadLeft(3)} > {playerName}@{serverId}");
                //Found some duplicate friend entries in dat file. So wired???
                //Use hashSet to remove duplicate result
                _friendList.Add(new FriendListEntry
                {
                    playerName = playerName,
                    homeWorldServer = serverId
                });
                offset += 48;
            }
        }

        public override string ToString()
        {
            var contactList = _recentContactList.Select(entry => entry.playerName + "@" + entry.homeWorldServer).ToArray();
            var friendList = _friendList.Select(entry => entry.playerName + "@" + entry.homeWorldServer).ToArray();
            return "Recent Contact List : \n" + Strings.Join(contactList, ", ") + "\nFriend List:\n" + Strings.Join(friendList, ", ");
        }
    }
}