using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NoName.Memory
{
    public struct Box<T> : IDisposable, ICloneable<Box<T>>, IMoveable<Box<T>>
        where T : unmanaged
    {
        public ref T Ref
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<T>(_ptr);
                }
            }
        }

        private unsafe T* _ptr;

        public Box(in T value)
        {
            unsafe
            {
                _ptr = (T*) Marshal.AllocHGlobal(sizeof(T));
                *_ptr = value;
            }
        }

        public Box<T> Clone()
        {
            unsafe
            {
                var instance = new Box<T>
                {
                    _ptr = (T*) Marshal.AllocHGlobal(sizeof(T))
                };
                *(instance._ptr) = *_ptr;
                return instance;
            }
        }

        public Box<T> Move()
        {
            unsafe
            {
                var box = new Box<T> { _ptr = _ptr };
                _ptr = (T*)IntPtr.Zero;
                return box;
            }
        }

        public void Dispose()
        {
            unsafe
            {
                if (_ptr == (T*) IntPtr.Zero) return;
                Marshal.FreeHGlobal((IntPtr) _ptr);
                _ptr = (T*) IntPtr.Zero;
            }
        }

        public static implicit operator Box<T>(in T value) => new(value);
        public static explicit operator T(in Box<T> box) => box.Ref;
    }
}
