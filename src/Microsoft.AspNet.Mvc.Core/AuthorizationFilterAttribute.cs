// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class AuthorizationFilterAttribute :
        Attribute, IAsyncAuthorizationFilter, IAuthorizationFilter, IOrderedFilter
    {
        public int Order { get; set; }

#pragma warning disable 1998
        public virtual async Task OnAuthorizationAsync(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            OnAuthorization(context);
        }
#pragma warning restore 1998

        public virtual void OnAuthorization(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }

        protected virtual bool HasAllowAnonymous(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Filters.Any(item => item is IAllowAnonymous);
        }

        protected virtual void Fail(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = new HttpUnauthorizedResult();
        }
    }
}
