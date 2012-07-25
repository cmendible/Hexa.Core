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

        private readonly bool eagerLoad;
        private readonly Func<Pool<T>, T> factory;
        private readonly Queue<T> queue;
        private readonly Semaphore sync;
        private readonly bool usingExpirableObjects;

        private bool disposed;

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

            this.factory = factory;
            this.sync = new Semaphore(size, size);
            this.queue = new Queue<T>(size);
            this.eagerLoad = eagerLoad;

            if (this.eagerLoad)
            {
                this.PreloadItems(size);
            }

            this.usingExpirableObjects = typeof(IObjectWithExpiration<T>).IsAssignableFrom(typeof(T));
        }

        #endregion Constructors

        #region Properties

        public bool IsDisposed
        {
            get
            {
                return this.disposed;
            }
        }

        #endregion Properties

        #region Methods

        public T Acquire()
        {
            this.sync.WaitOne();
            lock (this.queue)
            {
                T item = default(T);
                if (!this.eagerLoad)
                {
                    if (this.queue.Count > 0)
                    {
                        item = this.queue.Dequeue();
                    }
                    else
                    {
                        item = this.factory(this);
                    }
                }
                else
                {
                    item = this.queue.Dequeue();
                }

                if (this.usingExpirableObjects)
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
                            item = this.factory(this);
                        }
                    }
                }
                return item;
            }
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        public void Release(T item)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(item);
            }
            this.sync.Release();
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    lock (this.queue)
                    {
                        while (this.queue.Count > 0)
                        {
                            IDisposable disposable = (IDisposable)this.queue.Dequeue();
                            disposable.Dispose();
                        }
                    }
                }
                this.sync.Close();

                this.disposed = true;
            }
        }

        private void PreloadItems(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T item = this.factory(this);
                this.queue.Enqueue(item);
            }
        }

        #endregion Methods
    }
}