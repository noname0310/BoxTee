namespace NoName.Memory
{
    internal struct RcData
    {
        public int StrongCount;
        public int WeakCount;

        public override string ToString() => $"(StrongCount: {StrongCount}, WeakCount: {WeakCount})";
    }
}
