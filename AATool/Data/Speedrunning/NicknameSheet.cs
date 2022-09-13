using System.Collections.Generic;
using AATool.Net;

namespace AATool.Data.Speedrunning
{
    public class NicknameSheet : Spreadsheet
    {
        private readonly int nickNameCol; 
        private readonly int realNameCol;
        private readonly int uuidCol;

        private NicknameSheet(string csv) : base (csv, "leaderboard_names")
        {
            //find column headers
            this.nickNameCol = this.Find("name", "nickname", "nick", "preferred").X;
            this.realNameCol = this.Find("ign", "ingame name", "in-game name", "minecraft name", "mojang name").X;
            this.uuidCol = this.Find("uuid", "guid").X;

            this.IsValid = this.nickNameCol >= 0 && (this.realNameCol >= 0 || this.uuidCol >= 0);
        }

        public static bool TryParse(string csv, out NicknameSheet sheet)
        {
            sheet = new NicknameSheet(csv);
            return sheet.IsValid;
        }

        public void GetMappings(out Dictionary<string, string> realNames, 
            out Dictionary<string, string> nickNames, 
            out Dictionary<string, Uuid> identities)
        {
            realNames = new Dictionary<string, string>();
            nickNames = new Dictionary<string, string>();
            identities = new Dictionary<string, Uuid>();
            for (int i = 1; i < this.Rows.Length; i++)
            {
                if (this.TryGetNickname(i, out string nick))
                {
                    if (this.TryGetUuid(i, out Uuid uuid))
                    {
                        identities[nick.ToLower()] = uuid;
                        nickNames[uuid.String] = nick;
                    }

                    if (this.TryGetRealName(i, out string real))
                    {
                        if (uuid != Uuid.Empty)
                        {
                            identities[real.ToLower()] = uuid;
                            realNames[uuid.String] = real;
                        }
                        realNames[nick.ToLower()] = real;
                        nickNames[real.ToLower()] = nick;
                    }
                }
            }
        }

        public bool TryGetUuid(int index, out Uuid uuid)
        {
            if (this.TryGetCell(index, this.uuidCol, out string uuidString))
                uuidString = uuidString.Trim();
            return Uuid.TryParse(uuidString, out uuid);
        }

        public bool TryGetNickname(int index, out string nickName)
        {
            if (this.TryGetCell(index, this.nickNameCol, out nickName))
                nickName = nickName.Trim();
            return !string.IsNullOrEmpty(nickName);
        }

        public bool TryGetRealName(int index, out string realName)
        {
            if (this.TryGetCell(index, this.realNameCol, out realName))
                realName = realName.Trim();
            return !string.IsNullOrEmpty(realName);
        }
    }
}
