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

namespace Hexa.Core.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml;

    /// <summary>
    /// Used to resolve xml name spaces.
    /// </summary>
    public class SchemaResolver : XmlResolver
    {
        #region Fields

        private readonly Dictionary<string, byte[]> _schemas;

        #endregion Fields

        #region Constructors

        public SchemaResolver(Dictionary<string, byte[]> schemas)
        {
            _schemas = schemas;
        }

        #endregion Constructors

        #region Properties

        public override ICredentials Credentials
        {
            set { }
        }

        #endregion Properties

        #region Methods

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            string name = absoluteUri.AbsoluteUri.Split(new[] {'/'}).Last();
            byte[] stream = _schemas[name];
            if (stream != null)
            {
                return new MemoryStream(stream);
                ;
            }
            var resolver = new XmlUrlResolver();
            return resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return base.ResolveUri(baseUri, relativeUri);
        }

        #endregion Methods
    }
}