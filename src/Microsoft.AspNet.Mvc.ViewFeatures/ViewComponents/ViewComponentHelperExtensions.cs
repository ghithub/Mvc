// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    public static class ViewComponentHelperExtensions
    {
        public static HtmlString Invoke<TComponent>(this IViewComponentHelper helper,
            params object[] args)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Invoke(typeof(TComponent), args);
        }

        public static void RenderInvoke<TComponent>(this IViewComponentHelper helper,
            params object[] args)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            helper.RenderInvoke(typeof(TComponent), args);
        }

        public static async Task<HtmlString> InvokeAsync<TComponent>(this IViewComponentHelper helper,
            params object[] args)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return await helper.InvokeAsync(typeof(TComponent), args);
        }

        public static async Task RenderInvokeAsync<TComponent>(this IViewComponentHelper helper,
            params object[] args)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            await helper.RenderInvokeAsync(typeof(TComponent), args);
        }
    }
}
