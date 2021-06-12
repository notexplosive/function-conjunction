using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    public class CurveData
    {
        public float heightDomain = 2;
        public float widthDomain = MathF.PI * 2;

        public CurveData(float widthDomain, float heightDomain)
        {
            this.heightDomain = heightDomain;
            this.widthDomain = widthDomain;
        }
    }
}
