namespace NoName.Memory
{
    public interface IMoveable<out T>
    {
        public T Move();
    }
}