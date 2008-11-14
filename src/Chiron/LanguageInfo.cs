/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Text;

namespace Chiron {
    class LanguageInfo {
        internal readonly string[] Extensions;
        internal readonly string[] Assemblies;
        internal readonly string LanguageContext;

        public LanguageInfo(string[] extensions, string[] assemblies, string languageContext) {
            Extensions = extensions;
            Assemblies = assemblies;
            LanguageContext = languageContext;
        }

        public string GetContextAssemblyName() {
            return AssemblyName.GetAssemblyName(Chiron.TryGetAssemblyPath(Assemblies[0])).FullName;
        }

        public string GetExtensionsString() {
            StringBuilder str = new StringBuilder();
            foreach (string ext in Extensions) {
                if (str.Length > 0) {
                    str.Append(",");
                }
                str.Append(ext + ",." + ext);
            }
            return str.ToString();
        }
    }

    public class LanguageSection : IConfigurationSectionHandler {
        public object Create(object parent, object configContext, XmlNode section) {
            Dictionary<string, LanguageInfo> languages = new Dictionary<string, LanguageInfo>();
            char[] splitChars = new char[] { ' ', '\t', ',', ';', '\r', '\n' };

            foreach (XmlElement elem in ((XmlElement)section).GetElementsByTagName("Language")) {
                LanguageInfo info = new LanguageInfo(
                    elem.GetAttribute("extensions").Split(splitChars, StringSplitOptions.RemoveEmptyEntries),
                    elem.GetAttribute("assemblies").Split(splitChars, StringSplitOptions.RemoveEmptyEntries),
                    elem.GetAttribute("languageContext")
                );

                foreach (string ext in info.Extensions) {
                    languages["." + ext.ToLower()] = info;
                }
            }

            return languages;
        }
    }
}
