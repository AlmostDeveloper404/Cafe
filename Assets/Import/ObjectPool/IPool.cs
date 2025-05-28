using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public interface Pool<T>
    {
        T Pull();

        void Push(T t);
    }
}

