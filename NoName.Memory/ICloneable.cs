namespace NoName.Memory
{
    public interface ICloneable<out T>
    {
        public T Clone();
    }
}