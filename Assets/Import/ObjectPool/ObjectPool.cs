using UnityEngine;
using System.Collections.Generic;
using System;
using Zenject;


namespace Main
{
    public class ObjectPool<T> : Pool<T> where T : MonoBehaviour, IPoolable<T>
    {
        private Action<T> _push;
        private Action<T> _pull;
        private Stack<T> pooledObjects = new Stack<T>();
        private GameObject _prefab;

        private DiContainer _diContainer;


        public ObjectPool(GameObject pooledObject, DiContainer diContainer)
        {
            _prefab = pooledObject;
            _diContainer = diContainer;
        }

        public ObjectPool(GameObject pooledObject, DiContainer diContainer, int numToSpawn)
        {
            _prefab = pooledObject;
            _diContainer = diContainer;
            Spawn(numToSpawn);
        }

        public ObjectPool(GameObject pooledObject, int numToSpawn = 0)
        {
            _prefab = pooledObject;
            Spawn(numToSpawn);
        }

        public ObjectPool(GameObject pooledObject, Action<T> pullObject, Action<T> pushObject, int numToSpawn = 0)
        {
            _prefab = pooledObject;
            _pull = pullObject;
            _push = pushObject;
            Spawn(numToSpawn);
        }

        public int PooledCount
        {
            get
            {
                return pooledObjects.Count;
            }
        }

        public T Pull()
        {
            T t;
            if (PooledCount > 0)
                t = pooledObjects.Pop();
            else
                t = GameObject.Instantiate(_prefab).GetComponent<T>();

            t.gameObject.SetActive(true); //ensure the object is on
            t.Initialize(Push);

            //allow default behavior and turning object back on
            _pull?.Invoke(t);

            return t;
        }

        public T PullZenject()
        {
            T t;
            if (PooledCount > 0)
                t = pooledObjects.Pop();
            else
            {
                t = _diContainer.InstantiatePrefab(_prefab).GetComponent<T>();
            }


            t.gameObject.SetActive(true); //ensure the object is on
            t.Initialize(Push);

            //allow default behavior and turning object back on
            _pull?.Invoke(t);

            return t;
        }

        public T Pull(Vector3 position)
        {
            T t = Pull();
            t.transform.position = position;
            return t;
        }

        public T Pull(Vector3 position, Quaternion rotation)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.rotation = rotation;
            return t;
        }

        public GameObject PullGameObject()
        {
            return Pull().gameObject;
        }

        public GameObject PullGameObject(Vector3 position)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        public void Push(T t)
        {
            pooledObjects.Push(t);

            //create default behavior to turn off objects
            _push?.Invoke(t);

            t.gameObject.SetActive(false);
        }

        private void Spawn(int number)
        {
            T t;

            for (int i = 0; i < number; i++)
            {
                t = GameObject.Instantiate(_prefab).GetComponent<T>();
                pooledObjects.Push(t);
                t.gameObject.SetActive(false);
            }
        }
    }
}


