using System;

namespace Main
{
    public interface IPoolable<T>
    {
        void Initialize(Action<T> returnAction);

        void ReturnToPool();
    }
}
