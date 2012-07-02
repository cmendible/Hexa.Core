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

using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Hexa.Core.Xml
{
	public static class Transformer
	{
        public static byte[] Transform(byte[] inputDocument, byte[] xsl)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;

            using (MemoryStream memXsl = new MemoryStream(xsl))
            {
                xslt.Load(XmlReader.Create(new MemoryStream(xsl)));
            }

            using (MemoryStream memOut = new MemoryStream())
            {
                using (MemoryStream memXml = new MemoryStream(inputDocument))
                {
                    xslt.Transform(XmlReader.Create(memXml, settings), XmlWriter.Create(memOut, xslt.OutputSettings));
                }
                return memOut.ToArray();
            }
        }
	}
}
