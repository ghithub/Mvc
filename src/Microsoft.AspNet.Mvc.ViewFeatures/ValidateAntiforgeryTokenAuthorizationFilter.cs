// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Antiforgery;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    public class ValidateAntiforgeryTokenAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IAntiforgery _antiforgery;

        public ValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery)
        {
            if (antiforgery == null)
            {
                throw new ArgumentNullException(nameof(antiforgery));
            }

            _antiforgery = antiforgery;
        }

        public async Task OnAuthorizationAsync(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await _antiforgery.ValidateRequestAsync(context.HttpContext);
        }
    }
}