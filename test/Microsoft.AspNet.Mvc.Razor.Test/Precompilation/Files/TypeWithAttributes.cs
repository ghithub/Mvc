// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    [TargetElement("img", Attributes = AppendVersionAttributeName + "," + SrcAttributeName)]
    [TargetElement("image", Attributes = SrcAttributeName)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TypeWithAttributes
    {
        private const string AppendVersionAttributeName = "asp-append-version";
        private const string SrcAttributeName = "src";

        [HtmlAttributeName(SrcAttributeName)]
        public string Src { get; set; }

        [HtmlAttributeName(AppendVersionAttributeName, DictionaryAttributePrefix = "prefix")]
        public bool AppendVersion { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ViewContext ViewContext { get; set; }

        protected IHostingEnvironment HostingEnvironment { get; }
    }
}
