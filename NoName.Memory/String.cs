using System;
using System.Runtime.InteropServices;

namespace NoName.Memory
{
    public struct String : IDisposable, ICloneable<String>, IMoveable<String>
    {
        public char this[int index] {
            get
            {
                if (Length <= index)
                    throw new IndexOutOfRangeException();
                unsafe
                {
                    return Source[index];
                }
            }
        }

        public int Length { get; private set; }
        public unsafe char* Source { get; private set; }
        private bool NeedDispose { get; init; }

        public String(string str)
        {
            unsafe
            {
                Source = (char*) Marshal.AllocHGlobal(str.Length * sizeof(char));
            }
            Length = str.Length;
            NeedDispose = true;
        }

        public unsafe String(char* source, int length)
        {
            Source = source;
            Length = length;
            NeedDispose = false;
        }

        public String(String str)
        {
            unsafe
            {
                Source = (char*)Marshal.AllocHGlobal(str.Length * sizeof(char));
            }
            Length = str.Length;
            NeedDispose = true;
        }

        public String Clone() => new(this);

        public String Move()
        {
            unsafe
            {
                var instance = new String
                {
                    Length = Length,
                    Source = Source,
                    NeedDispose = NeedDispose
                };
                Length = 0;
                Source = (char*)IntPtr.Zero;
                return instance;
            }
        }

        public override string ToString() => (string)this;

        public void Dispose()
        {
            if (!NeedDispose)
                return;
            unsafe
            {
                if (Source == (char*) IntPtr.Zero)
                    return;
                Marshal.FreeHGlobal((IntPtr) Source);
                Source = (char*) IntPtr.Zero;
                Length = 0;
            }
        }

        public static implicit operator String(string str) => new(str);
        public static explicit operator string(String str)
        {
            unsafe
            {
                return new string(str.Source, 0, str.Length);
            }
        }
    }
}
