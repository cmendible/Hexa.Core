#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================
// Inspired on Pool<T> From http://pastebin.com/he1fYC29

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Hexa.Core.Pooling
{
    public interface IObjectWithExpiration<T> : IDisposable
    {
        DateTime TimeOut { get; set; }
        bool IsExpired { get; }
    }

    public class Pool<T> : IDisposable
    {
        private bool _isDisposed;
        private Func<Pool<T>, T> _factory;
        private Queue<T> _queue;
        private Semaphore _sync;
        private bool _usingExpirableObjects;
        private bool _eagerLoad;

        public Pool(int size, Func<Pool<T>, T> factory) : this(size, factory, false)
        {
        }

        public Pool(int size, Func<Pool<T>, T> factory, bool eagerLoad)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", size,
                    "Argument 'size' must be greater than zero.");
            if (factory == null)
                throw new ArgumentNullException("factory");

            _factory = factory;
            _sync = new Semaphore(size, size);
            _queue = new Queue<T>(size);
            _eagerLoad = eagerLoad;

            if (_eagerLoad)
            {
                _PreloadItems(size);
            }

            _usingExpirableObjects = typeof(IObjectWithExpiration<T>).IsAssignableFrom(typeof(T));
        }

        public T Acquire()
        {
            _sync.WaitOne();
            lock (_queue)
            {
                var item = default(T);
                if (!_eagerLoad)
                {
                    if (_queue.Count > 0)
                        item = _queue.Dequeue();
                    else
                        item  = _factory(this);
                }
                else
                {
                    item = _queue.Dequeue();
                }

                if (_usingExpirableObjects)
                {
                    var expirableObject = item as IObjectWithExpiration<T>;
                    if (expirableObject.IsExpired)
                    {
                        try
                        {
                            expirableObject.Dispose();
                        }
                        finally
                        {
                            item = _factory(this);
                        }
                    }
                }
                return item;
            }
        }

        public void Release(T item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }
            _sync.Release();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (_queue)
                {
                    while (_queue.Count > 0)
                    {
                        IDisposable disposable = (IDisposable)_queue.Dequeue();
                        disposable.Dispose();
                    }
                }
            }
            _sync.Close();
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        private void _PreloadItems(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T item = _factory(this);
                _queue.Enqueue(item);
            }
        }
    }

}