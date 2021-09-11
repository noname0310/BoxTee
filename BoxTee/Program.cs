using System;
using NoName.Memory;

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

            Console.WriteLine($"non boxed {a}");
            Console.WriteLine($"boxed {boxedA.Ref}");

            using var moved = boxedA.Move();
            Console.WriteLine($"moved {moved.Ref}");

            using var rcA = (Rc<Foo>) a;
            Console.WriteLine($"{rcA} {rcA.Ref}");

            using var rcCloned = rcA.Clone();
            Console.WriteLine($"{rcCloned} {rcCloned.Ref}");

            using var weak = rcA.Downgrade();
            if (weak.TryGet(out var value))
            {
                Console.WriteLine(value);
            }
        }
    }

    internal struct Foo
    {
        public int A { get; set; }
        public long B { get; set; }

        public override string ToString() => $"(A: {A}, B: {B})";
    }
}
