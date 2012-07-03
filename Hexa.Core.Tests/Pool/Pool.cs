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

#endregion

namespace Hexa.Core.Tests
{
    using System;
    using NUnit.Framework;
    using Pooling;

    public class ExpirableEntity : IObjectWithExpiration<ExpirableEntity>
    {
        #region IObjectWithExpiration<ExpirableEntity> Members

        public DateTime TimeOut
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsExpired
        {
            get { return true; }
        }

        public void Dispose()
        {
        }

        #endregion
    }

    [TestFixture]
    public class ObjectPoolTests
    {
        private Pool<ExpirableEntity> _Pool;
        private ExpirableEntity _objectFromPool;

        [Test]
        public void CreatePool()
        {
            this._Pool = new Pool<ExpirableEntity>(10, (p) => { return new ExpirableEntity(); }, true);
            ExpirableEntity obj = this._Pool.Acquire();

            Assert.IsNotNull(obj);
            Assert.AreEqual(typeof (ExpirableEntity), obj.GetType());

            this._objectFromPool = obj;
        }

        [Test]
        public void ReleaseObject()
        {
            this._Pool.Release(this._objectFromPool);
        }
    }
}