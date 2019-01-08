using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MultiDword
    {
        [FieldOffset(0)]
        private float floatVal;

        [FieldOffset(0)]
        private int intVal;

        public static implicit operator MultiDword(int a) => new MultiDword() { Int = a };
        public static implicit operator MultiDword(float a) => new MultiDword() { Float = a };
        public static implicit operator int(MultiDword a) => a.Int;
        public static implicit operator float(MultiDword a) => a.Float;

        public float Float
        {
            get => floatVal;
            set => floatVal = value;
        }

        public int Int
        {
            get => intVal;
            set => intVal = value;
        }
    }
}
