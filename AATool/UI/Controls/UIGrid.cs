using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIGrid : UIControl
    {
        public List<Size> Columns;
        public List<Size> Rows;
        public List<bool> CollapsedColumns;
        public List<bool> CollapsedRows;
        protected Rectangle[,] cells;

        public UIGrid()
        {
            Columns = new List<Size>();
            Rows = new List<Size>();
            CollapsedColumns = new List<bool>();
            CollapsedRows = new List<bool>();
            DrawMode = DrawMode.ChildrenOnly;
        }

        public void CollapseRow(int row)
        {
            if (row >= 0 && row < Rows.Count)
                CollapsedRows[row] = true;
            ScaleTo(new Point(Width, Height - Rows[row].GetAbsoluteInt(Height)));
            ResizeCells();
        }

        public void ExpandRow(int row)
        {
            if (row >= 0 && row < Rows.Count)
                CollapsedRows[row] = false;
            ScaleTo(new Point(Width, Height + Rows[row].GetAbsoluteInt(Height)));
            ResizeCells();
        }

        public void CollapseCol(int col)
        {
            if (col >= 0 && col < Columns.Count)
                CollapsedColumns[col] = true;
            ScaleTo(new Point(Width - Columns[col].GetAbsoluteInt(Width), Height));
            ResizeCells();
        }

        public void ExpandCol(int col)
        {
            if (col >= 0 && col < Columns.Count)
                CollapsedColumns[col] = false;
            ScaleTo(new Point(Width + Columns[col].GetAbsoluteInt(Width), Height));
            ResizeCells();
        }

        private void ResizeCells()
        {
            //if no rows/columns exist, create one with 100% relative size
            if (Rows.Count == 0)
                Rows.Add(new Size(1, SizeMode.Relative));

            if (Columns.Count == 0)
                Columns.Add(new Size(1, SizeMode.Relative));

            int totalAbsoluteWidth = 0;
            int totalAbsoluteHeight = 0;

            //set up row lists
            foreach (var row in Rows)
            {
                if (row.Mode == SizeMode.Absolute)
                    totalAbsoluteHeight += (int)row.InternalValue;
                CollapsedRows.Add(false);
            }

            //set up column lists
            foreach (var col in Columns)
            { 
                if (col.Mode == SizeMode.Absolute)
                    totalAbsoluteWidth += (int)col.InternalValue;
                CollapsedColumns.Add(false);
            }
            
            cells = new Rectangle[Rows.Count, Columns.Count];

            double remainderY = 0;
            int y = ContentRectangle.Y;
            for (int r = 0; r < Rows.Count; r++)
            {
                //skip row if collapsed
                if (CollapsedRows[r])
                    continue;

                double remainderX = 0;
                int x = ContentRectangle.X;
                int height = Rows[r].GetAbsoluteInt(ContentRectangle.Height - totalAbsoluteHeight);
                remainderY += Rows[r].GetAbsoluteDouble(ContentRectangle.Height - totalAbsoluteHeight) - height;
                height += (int)remainderY;
                remainderY -= (int)remainderY;
                for (int c = 0; c < Columns.Count; c++)
                {
                    //skip column if collapsed
                    if (CollapsedColumns[c])
                        continue;

                    int width = Columns[c].GetAbsoluteInt(ContentRectangle.Width - totalAbsoluteWidth);
                    remainderX += Columns[c].GetAbsoluteDouble(ContentRectangle.Width - totalAbsoluteWidth) - width;
                    width += (int)remainderX;
                    remainderX -= (int)remainderX;

                    //set rectangle at row and column to appropriate size
                    cells[r, c] = new Rectangle(x, y, width, height);
                    x += width;
                }
                y += height;
            }

            ResizeChildren();
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            ResizeThis(rectangle);
            ResizeCells();
        }

        public override void ResizeChildren()
        {
            foreach (var child in Children)
            {
                if (child.Row >= Rows.Count || child.Column >= Columns.Count)
                    continue;

                if (child.Row < 0 || child.Column < 0)
                    continue;
                
                //collapse or expand child control to match cell state
                if (CollapsedRows[child.Row] || CollapsedColumns[child.Column])
                {
                    child.Collapse();
                    continue;
                }
                else
                    child.Expand();

                //conform child control size to cell size
                Rectangle rect = cells[child.Row, child.Column];
                int width = 0;
                for (int c = child.Column; c < Math.Min(child.Column + child.ColumnSpan, Columns.Count); c++)
                    width += cells[child.Row, c].Width;

                int height = 0;
                for (int r = child.Row; r < Math.Min(child.Row + child.RowSpan, Rows.Count); r++)
                    height += cells[r, child.Column].Height;

                child.ResizeRecursive(new Rectangle(rect.X, rect.Y, width, height));
            }
        }

        public override void DrawThis(Display display)
        {
            base.DrawThis(display);
            foreach (var cell in cells)
                display.DrawRectangle(cell, Color.Green, Color.Lime, 1);
        }

        public override void DrawDebugRecursive(Display display)
        {
            base.DrawDebugRecursive(display);
            foreach (var cell in cells)
            {
                display.DrawRectangle(new Rectangle(cell.Left, cell.Top, cell.Width, 1),                DebugColor * 0.5f);
                display.DrawRectangle(new Rectangle(cell.Right - 1, cell.Top + 1, 1, cell.Height - 2),  DebugColor * 0.5f);
                display.DrawRectangle(new Rectangle(cell.Left + 1, cell.Bottom - 1, cell.Width - 1, 1), DebugColor * 0.5f);
                display.DrawRectangle(new Rectangle(cell.Left, cell.Top + 1, 1, cell.Height - 1),       DebugColor * 0.5f);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);

            Rows = new List<Size>();
            XmlNode rowNode = node.SelectSingleNode("rows");
            if (rowNode != null && rowNode.HasChildNodes)
            {
                foreach (XmlNode row in rowNode.ChildNodes)
                    Rows.Add(ParseAttribute(row, "height", new Size(1, SizeMode.Relative)));
            }
            else
                Rows.Add(new Size(1, SizeMode.Relative));

            Columns = new List<Size>();
            XmlNode colNode = node.SelectSingleNode("columns");
            if (colNode != null && colNode.HasChildNodes)
            {
                foreach (XmlNode col in colNode.ChildNodes)
                    Columns.Add(ParseAttribute(col, "width", new Size(1, SizeMode.Relative)));
            }
            else
                Columns.Add(new Size(1, SizeMode.Relative));
        }
    }
}
