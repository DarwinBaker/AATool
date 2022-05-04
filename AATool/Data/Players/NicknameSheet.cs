using System.Collections.Generic;

namespace AATool.Data.Players
{
    public class NicknameSheet : Spreadsheet
    {
        private readonly int nickNameCol; 
        private readonly int realNameCol;

        private NicknameSheet(string csv) : base (csv, "leaderboard_names")
        {
            //find column headers
            this.nickNameCol = this.Find("name", "nickname", "nick", "preferred").X;
            this.realNameCol = this.Find("ign", "ingame name", "in-game name", "minecraft name", "mojang name").X;

            this.IsValid = this.nickNameCol >= 0 && this.realNameCol >= 0;
        }

        public static bool TryParse(string csv, out NicknameSheet sheet)
        {
            sheet = new NicknameSheet(csv);
            return sheet.IsValid;
        }

        public void GetMappings(out Dictionary<string, string> realNames, out Dictionary<string, string> nickNames)
        {
            realNames = new Dictionary<string, string>();
            nickNames = new Dictionary<string, string>();
            for (int i = 1; i < this.Rows.Length; i++)
            {
                if (this.TryGetNickname(i, out string nick) && this.TryGetRealName(i, out string real))
                {
                    realNames[nick.ToLower()] = real;
                    nickNames[real.ToLower()] = nick;
                }
            }
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
