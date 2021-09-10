using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BoxTee
{
    internal class Program
    {
        private static void Main()
        {
            var a = new Foo()
            {
                A = 1,
                B = 2038292938292
            };

            using var boxedA = (Box<Foo>) a;
            boxedA.Ref.A = 333;

            Console.WriteLine($"non boxed a={a.A} b={a.B}");
            Console.WriteLine($"boxed a={boxedA.Ref.A} b={boxedA.Ref.B}");

            using var moved = boxedA.Move();
            Console.WriteLine($"moved a={moved.Ref.A} b={moved.Ref.B}");

            using var rcA = (Rc<Foo>) a;
            Console.WriteLine($"sc: {rcA.StrongCount} a={rcA.Ref.A} b={rcA.Ref.B}");
            using var rcB = rcA.Clone();
            Console.WriteLine($"sc: {rcB.StrongCount} a={rcB.Ref.A} b={rcB.Ref.B}");
        }
    }

    internal struct Foo
    {
        public int A { get; set; }
        public long B { get; set; }
    }

    internal struct Box<T> : IDisposable
        where T : unmanaged
    {
        public ref T Ref
        {
            get
            {
                unsafe { return ref Unsafe.AsRef<T>(_ptr); }
            }
        }

        private unsafe T* _ptr;

        public Box(in T value)
        {
            unsafe
            {
                _ptr = (T*)Marshal.AllocHGlobal(sizeof(T));
                *_ptr = value;
            }
        }

        public Box<T> Move()
        {
            unsafe
            {
                var box = new Box<T> {_ptr = _ptr};
                _ptr = (T*) IntPtr.Zero;
                return box;
            }
        }

        public void Dispose()
        {
            unsafe
            {
                if (_ptr != (T*) IntPtr.Zero)
                    Marshal.FreeHGlobal((IntPtr) _ptr);
            }
        }

        public static implicit operator Box<T>(T value) => new(value);
        public static explicit operator T(Box<T> box) => box.Ref;
    }

    internal readonly struct Rc<T> : IDisposable
        where T : unmanaged
    {
        private struct Data<T2>
            where T2 : unmanaged
        {
            public int StrongCount;
            public int WeakCount;
            public T2 Value;
        }

        public int StrongCount
        {
            get
            {
                unsafe { return _ptr->StrongCount; }
            }
        }

        public int WeakCount
        {
            get
            {
                unsafe { return _ptr->WeakCount; }
            }
        }

        public ref T Ref
        {
            get
            {
                unsafe { return ref Unsafe.AsRef<T>(&(_ptr->Value)); }
            }
        }

        private readonly unsafe Data<T>* _ptr;

        public Rc(in T value)
        {
            unsafe
            {
                var intPtr = Marshal.AllocHGlobal(sizeof(Data<T>));
                _ptr = (Data<T>*) intPtr;
                _ptr->StrongCount = 1;
                _ptr->WeakCount = 0;
                _ptr->Value = value;
            }
        }

        public Rc<T> Clone()
        {
            unsafe
            {
                _ptr->StrongCount += 1;
                return this;
            }
        }

        public void Dispose()
        {
            unsafe
            {
                _ptr->StrongCount -= 1;
                if (_ptr->StrongCount <= 0)
                    Marshal.FreeHGlobal((IntPtr) _ptr);
            }
        }

        public static implicit operator Rc<T>(T value) => new(value);
        public static explicit operator T(Rc<T> box) => box.Ref;
    }
}
