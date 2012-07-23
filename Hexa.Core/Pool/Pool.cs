#region Header

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

#endregion Header

namespace Hexa.Core.Pooling
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public interface IObjectWithExpiration<T> : IDisposable
    {
        #region Properties

        bool IsExpired
        {
            get;
        }

        DateTime TimeOut
        {
            get;
            set;
        }

        #endregion Properties
    }

    public class Pool<T> : IDisposable
    {
        #region Fields

        private readonly bool _eagerLoad;
        private readonly Func<Pool<T>, T> _factory;
        private readonly Queue<T> _queue;
        private readonly Semaphore _sync;
        private readonly bool _usingExpirableObjects;

        private bool _isDisposed;

        #endregion Fields

        #region Constructors

        public Pool(int size, Func<Pool<T>, T> factory)
            : this(size, factory, false)
        {
        }

        public Pool(int size, Func<Pool<T>, T> factory, bool eagerLoad)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", size,
                                                      "Argument 'size' must be greater than zero.");
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this._factory = factory;
            this._sync = new Semaphore(size, size);
            this._queue = new Queue<T>(size);
            this._eagerLoad = eagerLoad;

            if (this._eagerLoad)
            {
                this._PreloadItems(size);
            }

            this._usingExpirableObjects = typeof(IObjectWithExpiration<T>).IsAssignableFrom(typeof(T));
        }

        #endregion Constructors

        #region Properties

        public bool IsDisposed
        {
            get
            {
                return this._isDisposed;
            }
        }

        #endregion Properties

        #region Methods

        public T Acquire()
        {
            this._sync.WaitOne();
            lock (this._queue)
            {
                T item = default(T);
                if (!this._eagerLoad)
                {
                    if (this._queue.Count > 0)
                    {
                        item = this._queue.Dequeue();
                    }
                    else
                    {
                        item = this._factory(this);
                    }
                }
                else
                {
                    item = this._queue.Dequeue();
                }

                if (this._usingExpirableObjects)
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
                            item = this._factory(this);
                        }
                    }
                }
                return item;
            }
        }

        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }
            this._isDisposed = true;
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (this._queue)
                {
                    while (this._queue.Count > 0)
                    {
                        var disposable = (IDisposable)this._queue.Dequeue();
                        disposable.Dispose();
                    }
                }
            }
            this._sync.Close();
        }

        public void Release(T item)
        {
            lock (this._queue)
            {
                this._queue.Enqueue(item);
            }
            this._sync.Release();
        }

        private void _PreloadItems(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T item = this._factory(this);
                this._queue.Enqueue(item);
            }
        }

        #endregion Methods
    }
}