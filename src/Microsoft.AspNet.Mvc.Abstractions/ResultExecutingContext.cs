// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    public class ResultExecutingContext : FilterContext
    {
        public ResultExecutingContext(
            ActionContext actionContext,
            IList<IFilterMetadata> filters,
            IActionResult result,
            object controller)
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

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            Result = result;
            Controller = controller;
        }

        public virtual object Controller { get; }

        public virtual IActionResult Result { get; set; }

        public virtual bool Cancel { get; set; }
    }
}
