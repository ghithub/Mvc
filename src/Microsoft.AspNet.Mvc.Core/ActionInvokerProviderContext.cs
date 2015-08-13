// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Core
{
    public class ActionInvokerProviderContext
    {
        public ActionInvokerProviderContext(ActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            ActionContext = actionContext;
        }

        public ActionContext ActionContext { get; }

        public IActionInvoker Result { get; set; }
    }
}
