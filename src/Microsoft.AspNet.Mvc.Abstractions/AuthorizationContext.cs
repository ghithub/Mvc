// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    public class AuthorizationContext : FilterContext
    {
        public AuthorizationContext(
            ActionContext actionContext,
            IList<IFilterMetadata> filters)
            : base(actionContext, filters)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }
        }

        public virtual IActionResult Result { get; set; }
    }
}
