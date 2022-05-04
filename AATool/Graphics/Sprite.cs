using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Graphics
{
    public class Sprite
    {
        public const char ResolutionFlag = '^';
        public const char FramesFlag = '$';
        public const char ColumnsDelimiter = 'x';
        public const char PaddingFlag = '~';
        public const int MaxAnimationColumns = 32;

        public Rectangle Source { get; protected set; }
        public Vector2 Origin { get; protected set; }

        public Point Offset => this.Source.Location;
        public int Width => this.Source.Width;
        public int Height => this.Source.Height;

        public Sprite(Rectangle source)
        {
            this.Source = source;
            this.Origin = new Vector2(source.Width / 2, source.Height / 2);
        }

        public static string ParseId(string fileName,
            out int padding,
            out int frames,
            out int columns,
            out decimal speed)
        {
            string key = fileName;
            ExtractAnimation(ref key, out frames, out columns, out speed);
            ExtractPadding(ref key, out padding);
            return key;
        }

        private static string ExtractAnimation(ref string key, out int frames, out int columns, out decimal speed)
        {
            frames = 0;
            columns = 0;
            speed = 1;
            int index = key.IndexOf(FramesFlag);
            if (index is -1)
                return key;

            //parse frame count and optional column count
            string[] tokens = key.Substring(index + 1).Split(ColumnsDelimiter);
            int.TryParse(tokens.FirstOrDefault(), out frames);
            if (tokens.Length > 2)
                decimal.TryParse(tokens[2], out speed);
            if (tokens.Length > 1)
                int.TryParse(tokens[1], out columns);
            else
                columns = Math.Min(frames, MaxAnimationColumns);

            //remove animation tag from key
            return key = key.Substring(0, index);
        }

        private static string ExtractPadding(ref string key, out int padding)
        {
            padding = 0;
            int index = key.IndexOf(PaddingFlag);
            if (index is -1)
                return key;

            int.TryParse(key.Substring(index + 1), out padding);

            //remove padding tag from key
            return key = key.Substring(0, index);
        }
    }
}
