using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tplan;

namespace FastImageTabMaker
{
    class CoordConverter
    {
        internal double a1, b1, c1;
        internal double a2, b2, c2;
        PointWList plist;
        internal CoordConverter(PointWList plist)
        {
            this.plist=plist;
        }
        internal void ConvertPointD2W(int sx, int sy, out double wx, out double wy)
        {
            wx = sx * a1 + sy * b1 + c1;
            wy = sx * a2 + sy * b2 + c2;

        }
        internal void Calculate()
        {
           if (plist.Count<3) return;
           PointW p1=plist[0];
           PointW p2=plist[1];
           PointW p3=plist[2];

           Matrix2 mtrs=new Matrix2(p1.x,p1.y,1,
                                   p2.x,p2.y,1,
                                   p3.x,p3.y,1); 
           Matrix2 mtrd=new Matrix2(p1.wx,p1.wy,1,
                                   p2.wx,p2.wy,1,
                                   p3.wx,p3.wy,1);
           mtrs.Invert();
           mtrs.Mul(mtrd);
           a1 = mtrs.els[0, 0];
           b1 = mtrs.els[0, 1];
           c1 = mtrs.els[0, 2];
           a2 = mtrs.els[1, 0];
           b2 = mtrs.els[1, 1];
           c2 = mtrs.els[1, 2];


        }

        internal bool NoCalc()
        {
            return (a1 == 0) && (b1 == 0) && (c1 == 0) && (a2 == 0) && (b2 == 0) && (c2 == 0);
        }
    }
}
