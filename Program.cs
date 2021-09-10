using System;
using System.Runtime.InteropServices;

namespace BoxTee
{
    internal class Program
    {
        private static unsafe void Main()
        {
            var a = new Foo()
            {
                A = 1,
                B = 2038292938292
            };

            using var boxedA = new Box<Foo>(a);
            boxedA.Ptr->A = 333;

            Console.WriteLine($"non boxed a={a.A} b={a.B}");
            Console.WriteLine($"boxed a={boxedA.Ptr->A} b={boxedA.Ptr->B}");

            using var moved = new Box<Foo>(boxedA);
            Console.WriteLine($"moved a={moved.Ptr->A} b={moved.Ptr->B}");
        }
    }

    internal struct Foo
    {
        public int A { get; set; }
        public long B { get; set; }
    }

    internal unsafe struct Box<T> : IDisposable
        where T : unmanaged
    {
        public T* Ptr { get; private set; }

        public Box(T value)
        {
            var intPtr = Marshal.AllocHGlobal(sizeof(T));
            Ptr = (T*)intPtr;
            Marshal.StructureToPtr(value, intPtr, true);
        }

        public Box(Box<T> box)
        {
            Ptr = box.Ptr;
            box.Ptr = (T*)IntPtr.Zero;
        }

        public void Dispose()
        {
            if (Ptr != (T*)IntPtr.Zero)
                Marshal.FreeHGlobal((IntPtr)Ptr);
        }
    }

    internal unsafe struct Rc<T> 
}
