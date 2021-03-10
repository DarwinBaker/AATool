using AATool.Graphics;

namespace AATool.UI.Controls
{
    class UIEnchantmentTable : UIPicture
    {
        private const string TEXTURE_READING = "enchantment_table_reading";
        private const string TEXTURE_OPENING = "enchantment_table_opening";
        private const string TEXTURE_CLOSING = "enchantment_table_closing";
        private const string TEXTURE_CLOSED  = "enchantment_table_closed";

        private Sprite open;
        private Sprite close;

        public UIEnchantmentTable()
        {
            Texture = TEXTURE_CLOSED;
            SpriteSheet.Sprites.TryGetValue(TEXTURE_OPENING, out open);
            SpriteSheet.Sprites.TryGetValue(TEXTURE_CLOSING, out close);
        }

        public void UpdateState(bool isReadingSave)
        {
            if (isReadingSave)
            {
                if (Texture == TEXTURE_CLOSED && SpriteSheet.GetFrameIndex(open.FrameCount, open.SourceRectangle.Width) == 0)
                    SetTexture(TEXTURE_OPENING);
                else if (Texture == TEXTURE_OPENING && SpriteSheet.GetFrameIndex(open.FrameCount, open.SourceRectangle.Width) == open.FrameCount - 1)
                    SetTexture(TEXTURE_READING);
            }
            else
            {
                if (Texture == null)
                    SetTexture(TEXTURE_CLOSED);
                else if (Texture == TEXTURE_READING && SpriteSheet.GetFrameIndex(close.FrameCount, close.SourceRectangle.Width) == 0)
                    SetTexture(TEXTURE_CLOSING);
                else if (Texture == TEXTURE_CLOSING && SpriteSheet.GetFrameIndex(close.FrameCount, close.SourceRectangle.Width) == close.FrameCount - 1)
                    SetTexture(TEXTURE_CLOSED);
            }
        }
    }
}
