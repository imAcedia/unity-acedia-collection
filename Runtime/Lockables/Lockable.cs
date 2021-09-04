
using UnityEngine;

namespace Acedia.Lockables
{
    // TODO Lockable<>: Integrate Lockables with IMultipleAttributes
    // TODO Lockable<>: More Lockables for default Unity Serializeable types
    [System.Serializable]
    public class Lockable<T>
    {
        [SerializeField] T value = default;
        [SerializeField] bool locked = false;

        private object key = null;

        public T Value { get => value; set => SetValue(value); }
        public bool Locked => locked;

        public Lockable() { }

        public Lockable(T value, bool locked = false)
        {
            this.value = value;
            this.locked = locked;
        }

        public virtual bool SetValue(T value, bool lockValue = false, object key = null)
        {
            if (locked) return false;

            this.value = value;
            locked = lockValue;
            this.key = key;
            return true;
        }

        public virtual bool Lock(object key = null)
        {
            if (locked) return false;
            locked = true;
            this.key = key;
            return true;
        }

        public virtual bool Unlock(object key = null)
        {
            if (!locked) return false;
            if (this.key != null && this.key != key)
                return false;

            locked = false;
            this.key = null;
            return true;
        }

        public static implicit operator T(Lockable<T> lockable)
        {
            return lockable.value;
        }
    }
}
