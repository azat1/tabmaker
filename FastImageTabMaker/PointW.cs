using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastImageTabMaker
{
    class PointW
    {
        internal int num;
        public int number
        {
            get { return num; }
            set { num = value; }
        }
        public int x
        {get;set;}

        public int y
        {get;set;}

        public double wx
        {get;set;}
        public double wy
        { get; set; }

        internal PointW(int num,int x, int y, double wx, double wy)
        {
            this.num=num;
            this.x = x;
            this.y = y;
            this.wx = wx;
            this.wy = wy;
        }
    }
}
