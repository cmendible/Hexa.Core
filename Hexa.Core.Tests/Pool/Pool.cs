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

using System;
using Hexa.Core.Pooling;
using NUnit.Framework;

namespace Hexa.Core.Tests
{
    public class ExpirableEntity : IObjectWithExpiration<ExpirableEntity>
    {
        public DateTime TimeOut
        {
            get
                {
                    throw new NotImplementedException();
                }
            set
                {
                    throw new NotImplementedException();
                }
        }

        public bool IsExpired
        {
            get
                {
                    return true;
                }
        }

        public void Dispose()
        {

        }
    }

    [TestFixture]
    public class ObjectPoolTests
    {
        Pool<ExpirableEntity> _Pool;
        ExpirableEntity _objectFromPool;

        [Test]
        public void CreatePool()
        {
            _Pool = new Pool<ExpirableEntity>(10, (p) => { return new ExpirableEntity(); }, true);
            var obj = _Pool.Acquire();

            Assert.IsNotNull(obj);
            Assert.AreEqual(typeof(ExpirableEntity), obj.GetType());

            _objectFromPool = obj;
        }

        [Test]
        public void ReleaseObject()
        {
            _Pool.Release(_objectFromPool);
        }
    }
}
