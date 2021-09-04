namespace Acedia
{
    // TODO ValueAccumulator<>: QA

    [System.Serializable]
    public class ValueAccumulator<T> : IValueAccumulator
    {
        public T value;

        public delegate void AccumulatorApplier(ValueAccumulator<T> acc);
        public AccumulatorApplier Applier { get; private set; }

        public void ApplyAccumulation() => Applier(this);

        public virtual TValue GetAccumulatedValue<TValue>()
        {
            if (!typeof(T).IsAssignableFrom(typeof(TValue)))
                throw new System.InvalidOperationException($"Cannot get value of type {typeof(TValue).Name} from {typeof(T).Name} Accumulator.");

            return (TValue)System.Convert.ChangeType(value, typeof(TValue));
        }

        public ValueAccumulator(AccumulatorApplier Applier)
        {
            value = default;
            this.Applier = Applier;
        }
    }
}
