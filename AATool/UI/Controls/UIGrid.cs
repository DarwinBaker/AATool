using System;
using System.Collections.Generic;
using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIGrid : UIControl
    {
        public List<bool> CollapsedColumns  { get; private set; }
        public List<bool> CollapsedRows     { get; private set; }
        public int ExpandedWidth            { get; private set; }
        public int ExpandedHeight           { get; private set; }

        protected List<Size> Columns        { get; private set; }
        protected List<Size> Rows           { get; private set; }

        protected Rectangle[,] CellRectangles;
        protected UIControl[,] CellContents;

        public UIGrid()
        {
            this.Columns          = new ();
            this.Rows             = new ();
            this.CollapsedColumns = new ();
            this.CollapsedRows    = new ();
            this.DrawMode         = DrawMode.ChildrenOnly;
        }

        public int GetExpandedWidth()
        {
            int total = 0;
            for (int col = 0; col < Math.Min(this.Columns.Count, this.CollapsedColumns.Count); col++)
            {
                if (!this.CollapsedColumns[col])
                    total += this.Columns[col].GetAbsoluteInt(this.Inner.Width);
            }
            return total;
        }

        public int GetExpandedHeight()
        {
            int total = 0;
            for (int row = 0; row < Math.Min(this.Rows.Count, this.CollapsedRows.Count); row++)
            {
                if (!this.CollapsedRows[row])
                    total += this.Rows[row].GetAbsoluteInt(this.Inner.Height);
            }
            return total;
        }

        public void CollapseRow(int row)
        {
            //collapse row and resize this
            if (this.CollapsedRows.Count > row && !this.CollapsedRows[row])
            {
                if (0 > row || row >= this.Rows.Count)
                    return;
                this.CollapsedRows[row] = true;
                this.ScaleTo(new Point(this.Width, this.Height - this.Rows[row].GetAbsoluteInt(this.Height)));
                this.ResizeCells();
            }
        }

        public void ExpandRow(int row)
        {
            //expand row and resize this
            if (this.CollapsedRows.Count > row && this.CollapsedRows[row])
            {
                if (0 > row || row >= this.Rows.Count)
                    return;
                this.CollapsedRows[row] = false;
                this.ScaleTo(new Point(this.Width, this.Height + this.Rows[row].GetAbsoluteInt(this.Height)));
                this.ResizeCells();
            }
        }

        public void CollapseCol(int col)
        {
            //collapse column and resize this
            if (this.CollapsedColumns.Count > col && !this.CollapsedColumns[col])
            {
                if (0 > col || col >= this.Columns.Count)
                    return;
                this.CollapsedColumns[col] = true;
                this.ScaleTo(new Point(this.Width - this.Columns[col].GetAbsoluteInt(this.Width), this.Height));
                this.ResizeCells();
            }
        }

        public void ExpandCol(int col)
        {
            //collapse column and resize this
            if (this.CollapsedColumns.Count > col && this.CollapsedColumns[col])
            {
                if (0 > col || col >= this.Columns.Count)
                    return;
                this.CollapsedColumns[col] = false;
                this.ScaleTo(new Point(this.Width + this.Columns[col].GetAbsoluteInt(this.Width), this.Height));
                this.ResizeCells();
            }
        }

        private void ResizeCells()
        {
            //if no rows/columns exist, create one with 100% relative size
            if (this.Rows.Count is 0)
                this.Rows.Add(new Size(1, SizeMode.Relative));

            if (this.Columns.Count is 0)
                this.Columns.Add(new Size(1, SizeMode.Relative));

            int totalAbsoluteWidth  = 0;
            int totalAbsoluteHeight = 0;

            //set up row lists
            foreach (Size row in this.Rows)
            {
                if (row.Mode is SizeMode.Absolute)
                    totalAbsoluteHeight += (int)row.InternalValue;
            }

            //set up column lists
            foreach (Size col in this.Columns)
            { 
                if (col.Mode is SizeMode.Absolute)
                    totalAbsoluteWidth += (int)col.InternalValue;
            }

            this.CellRectangles = new Rectangle[this.Rows.Count, this.Columns.Count];
            this.CellContents   = new UIControl[this.Rows.Count, this.Columns.Count];

            //keep track of remainder from rounding errors so we don't end up with gaps
            double remainderY = 0;
            int y = this.Inner.Y;
            for (int r = 0; r < this.Rows.Count; r++)
            {
                //skip row if collapsed
                if (this.CollapsedRows[r])
                    continue;

                //keep track of remainder from rounding errors so we don't end up with gaps
                double remainderX = 0;
                int x = this.Inner.X;
                int height  = this.Rows[r].GetAbsoluteInt(this.Inner.Height - totalAbsoluteHeight);
                remainderY += this.Rows[r].GetAbsoluteDouble(this.Inner.Height - totalAbsoluteHeight) - height;
                height     += (int)remainderY;
                remainderY -= (int)remainderY;

                for (int c = 0; c < this.Columns.Count; c++)
                {
                    //skip column if collapsed
                    if (this.CollapsedColumns[c])
                        continue;

                    int width   = this.Columns[c].GetAbsoluteInt(this.Inner.Width - totalAbsoluteWidth);
                    remainderX += this.Columns[c].GetAbsoluteDouble(this.Inner.Width - totalAbsoluteWidth) - width;
                    width      += (int)remainderX;
                    remainderX -= (int)remainderX;

                    //set rectangle at row and column to appropriate size
                    this.CellRectangles[r, c] = new Rectangle(x, y, width, height);
                    x += width;
                }
                y += height;
            }

            this.ResizeChildren();
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            this.ResizeThis(rectangle);
            this.ResizeCells();
        }

        public override void ResizeChildren()
        {
            foreach (UIControl child in this.Children)
            {
                //make sure row and column are valid
                if (child.Row < 0 || child.Row >= this.Rows.Count)
                    continue;
                if (child.Column < 0 || child.Column >= this.Columns.Count)
                    continue;

                //collapse or expand child control to match cell state
                if (this.CollapsedRows[child.Row] || this.CollapsedColumns[child.Column])
                {
                    child.Collapse();
                    continue;
                }
                else
                {
                    child.Expand();
                }

                //conform child control size to cell size
                Rectangle rectangle = this.CellRectangles[child.Row, child.Column];
                int width   = 0;
                int colSpan = Math.Min(child.Column + child.ColumnSpan, this.Columns.Count);
                for (int c = child.Column; c < colSpan; c++)
                    width += this.CellRectangles[child.Row, c].Width;

                int height  = 0;
                int rowSpan = Math.Min(child.Row + child.RowSpan, this.Rows.Count);
                for (int r = child.Row; r < rowSpan; r++)
                    height += this.CellRectangles[r, child.Column].Height;

                this.CellContents[child.Row, child.Column] = child;
                child.ResizeRecursive(new Rectangle(rectangle.X, rectangle.Y, width, height));
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            base.DrawThis(canvas);
            foreach (Rectangle cell in this.CellRectangles)
                canvas.DrawRectangle(cell, Config.Main.BackColor, Config.Main.BorderColor, 1);
        }

        public override void DrawDebugRecursive(Canvas canvas)
        {
            base.DrawDebugRecursive(canvas);
            foreach (Rectangle cell in this.CellRectangles)
            {
                //cell edges
                canvas.DrawRectangle(new Rectangle(cell.Left, cell.Top, cell.Width, 1),                
                    this.DebugColor * 0.5f, null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(cell.Right - 1, cell.Top + 1, 1, cell.Height - 2),  
                    this.DebugColor * 0.5f, null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(cell.Left + 1, cell.Bottom - 1, cell.Width - 1, 1), 
                    this.DebugColor * 0.5f, null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(cell.Left, cell.Top + 1, 1, cell.Height - 1),       
                    this.DebugColor * 0.5f, null, 0, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);

            this.Rows       = new ();
            XmlNode rowNode = node.SelectSingleNode("rows");
            if (rowNode is not null && rowNode.HasChildNodes)
            {
                foreach (XmlNode row in rowNode.ChildNodes)
                {
                    this.Rows.Add(Attribute(row, "height", new Size(1, SizeMode.Relative)));
                    this.CollapsedRows.Add(Attribute(row, "collapsed", false));
                }   
            }
            else
            {
                this.Rows.Add(new Size(1, SizeMode.Relative));
                this.CollapsedRows.Add(false);
            }

            this.Columns    = new ();
            XmlNode colNode = node.SelectSingleNode("columns");
            if (colNode is not null && colNode.HasChildNodes)
            {
                foreach (XmlNode col in colNode.ChildNodes)
                {
                    this.Columns.Add(Attribute(col, "width", new Size(1, SizeMode.Relative)));
                    this.CollapsedColumns.Add(Attribute(col, "collapsed", false));
                }
            }
            else
            {
                this.Columns.Add(new Size(1, SizeMode.Relative));
                this.CollapsedColumns.Add(false);
            }
        }
    }
}
