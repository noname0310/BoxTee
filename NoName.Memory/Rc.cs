using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NoName.Memory
{
    public readonly struct Rc<T> : IDisposable
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

        public ref T Ref
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<T>(Value);
                }
            }
        }

        public Rc(in T value)
        {
            unsafe
            {
                var dataPtr = Marshal.AllocHGlobal(sizeof(RcData));
                Data = (RcData*) dataPtr;
                Data->StrongCount = 1;
                Data->WeakCount = 0;
                var valuePtr = Marshal.AllocHGlobal(sizeof(T));
                Value = (T*) valuePtr;
                *Value = value;
            }
        }

        public Rc(in Weak<T> weak)
        {
            unsafe
            {
                Data = weak.Data;
                Value = weak.Value;
                Data->StrongCount += 1;
            }
        }

        public Rc<T> Clone()
        {
            unsafe
            {
                Data->StrongCount += 1;
                return this;
            }
        }

        public Weak<T> Downgrade() => new(this);

        public void Dispose()
        {
            unsafe
            {
                Data->StrongCount -= 1;
                if (Data->StrongCount > 0) return;
                Marshal.FreeHGlobal((IntPtr) Value);
                if (Data->WeakCount <= 0)
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

        public static implicit operator Rc<T>(in T value) => new(value);
        public static explicit operator T(in Rc<T> rc) => rc.Ref;
    }
}
