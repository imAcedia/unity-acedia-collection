namespace Acedia
{
    public interface IValueAccumulator
    {
        T GetAccumulatedValue<T>();
        void ApplyAccumulation();
    }
}
