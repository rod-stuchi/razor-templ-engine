using System;
using System.Drawing;
using static System.Console;

namespace RodStu.CMD
{
    public class ItemMark
    {
        private bool _check;

        public ItemMark(string item, bool check = false)
        {
            this.Item = item;
            this.Check = check;

            Write(" [  ");
            this.Left = Console.CursorLeft - 1;
            this.Top = Console.CursorTop;
            Write($"  ] {this.Item}\r\n");

        }

        public bool Check
        {
            get
            {
                return _check;
            }
            set
            {
                if (value)
                {
                    Point current = new Point(Console.CursorLeft, Console.CursorTop);
                    Console.SetCursorPosition(this.Left, this.Top);
                    Write("OK");
                    Console.SetCursorPosition(current.X, current.Y);
                }
                _check = value;
            }
        }
        public string Item { get; private set; }
        private int Left { get; set; }
        private int Top { get; set; }
    }
}
