// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    public class ActionExecutingContext : FilterContext
    {
        public ActionExecutingContext(
            ActionContext actionContext,
            IList<IFilterMetadata> filters,
            IDictionary<string, object> actionArguments,
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

            if (actionArguments == null)
            {
                throw new ArgumentNullException(nameof(actionArguments));
            }

            ActionArguments = actionArguments;
            Controller = controller;
        }

        public virtual IActionResult Result { get; set; }

        public virtual IDictionary<string, object> ActionArguments { get; }

        public virtual object Controller { get; }
    }
}
