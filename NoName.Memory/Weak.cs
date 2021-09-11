using System;
using System.Runtime.InteropServices;

namespace NoName.Memory
{
    public readonly struct Weak<T> : IDisposable
        where T : unmanaged
    {
        internal unsafe RcData* Data { get; }
        internal unsafe T* Value { get; }

        public int StrongCount
        {
            get
            {
                unsafe
                {
                    return Data->StrongCount;
                }
            }
        }

        public int WeakCount
        {
            get
            {
                unsafe
                {
                    return Data->WeakCount;
                }
            }
        }

        public Weak(in Rc<T> rc)
        {
            unsafe
            {
                Data = rc.Data;
                Value = rc.Value;
                Data->WeakCount += 1;
            }
        }

        public Rc<T> Upgrade() => new(this);

        public bool TryGet(out T value)
        {
            unsafe
            {
                if (1 <= Data->StrongCount)
                {
                    value = *Value;

                    return true;
                }
            }

            value = new T();
            return false;
        }

        public void Dispose()
        {
            unsafe
            {
                Data->WeakCount -= 1;
                if (Data->StrongCount <= 0 && Data->WeakCount <= 0)
                    Marshal.FreeHGlobal((IntPtr) Data);
            }
        }

        public override string ToString()
        {
            unsafe
            {
                return Data->ToString();
            }
        }

        public static implicit operator Weak<T>(in T value) => new(value);
    }
}
