#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================
// Inspired on Pool<T> From http://pastebin.com/he1fYC29

#endregion

namespace Hexa.Core.Pooling
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public interface IObjectWithExpiration<T> : IDisposable
    {
        DateTime TimeOut { get; set; }
        bool IsExpired { get; }
    }

    public class Pool<T> : IDisposable
    {
        private readonly bool _eagerLoad;
        private readonly Func<Pool<T>, T> _factory;
        private readonly Queue<T> _queue;
        private readonly Semaphore _sync;
        private readonly bool _usingExpirableObjects;
        private bool _isDisposed;

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

            _usingExpirableObjects = typeof (IObjectWithExpiration<T>).IsAssignableFrom(typeof (T));
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            if (typeof (IDisposable).IsAssignableFrom(typeof (T)))
            {
                lock (_queue)
                {
                    while (_queue.Count > 0)
                    {
                        var disposable = (IDisposable) _queue.Dequeue();
                        disposable.Dispose();
                    }
                }
            }
            _sync.Close();
        }

        #endregion

        public T Acquire()
        {
            _sync.WaitOne();
            lock (_queue)
            {
                T item = default(T);
                if (!_eagerLoad)
                {
                    if (_queue.Count > 0)
                        item = _queue.Dequeue();
                    else
                        item = _factory(this);
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