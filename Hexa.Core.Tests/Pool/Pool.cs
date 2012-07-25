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

#endregion Header

namespace Hexa.Core.Tests
{
    using System;

    using NUnit.Framework;

    using Pooling;

    public class ExpirableEntity : IObjectWithExpiration<ExpirableEntity>
    {
        #region Fields

        private bool disposed;

        #endregion Fields

        #region Properties

        public bool IsExpired
        {
            get
            {
                return true;
            }
        }

        public DateTime TimeOut
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

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
                // Note disposing has been done.
                disposed = true;

            }
        }

        #endregion Methods
    }

    [TestFixture]
    public class ObjectPoolTests : IDisposable
    {
        #region Fields

        private bool disposed;
        private ExpirableEntity objectFromPool;
        private Pool<ExpirableEntity> pool;

        #endregion Fields

        #region Methods

        [Test]
        public void CreatePool()
        {
            this.pool = new Pool<ExpirableEntity>(10, (p) => { return new ExpirableEntity(); }, true);
            ExpirableEntity obj = this.pool.Acquire();

            Assert.IsNotNull(obj);
            Assert.AreEqual(typeof(ExpirableEntity), obj.GetType());

            this.objectFromPool = obj;
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

        [Test]
        public void ReleaseObject()
        {
            this.pool.Release(this.objectFromPool);
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
                if (this.objectFromPool != null)
                {
                    this.objectFromPool.Dispose();
                }

                if (pool != null)
                {
                    this.pool.Dispose();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion Methods
    }
}