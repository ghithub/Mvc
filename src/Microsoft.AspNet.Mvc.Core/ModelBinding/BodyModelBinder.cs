// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    /// <summary>
    /// An <see cref="IModelBinder"/> which binds models from the request body using an <see cref="IInputFormatter"/>
    /// when a model has the binding source <see cref="BindingSource.Body"/>/
    /// </summary>
    public class BodyModelBinder : BindingSourceModelBinder
    {
        public static readonly string ModelStateKey = "$body";

        /// <summary>
        /// Creates a new <see cref="BodyModelBinder"/>.
        /// </summary>
        public BodyModelBinder()
            : base(BindingSource.Body)
        {
        }

        /// <inheritdoc />
        protected async override Task<ModelBindingResult> BindModelCoreAsync(
            [NotNull] ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.OperationBindingContext.HttpContext;

            var formatterContext = new InputFormatterContext(
                httpContext,
                bindingContext.ModelState,
                bindingContext.ModelType);
            var formatters = bindingContext.OperationBindingContext.InputFormatters;
            var formatter = formatters.FirstOrDefault(f => f.CanRead(formatterContext));

            if (formatter == null)
            {
                var unsupportedContentType = Resources.FormatUnsupportedContentType(
                    bindingContext.OperationBindingContext.HttpContext.Request.ContentType);
                bindingContext.ModelState.AddModelError(ModelStateKey, unsupportedContentType);

                // This model binder is the only handler for the Body binding source and it cannot run twice. Always
                // tell the model binding system to skip other model binders and never to fall back i.e. indicate a
                // fatal error.
                return new ModelBindingResult(ModelStateKey);
            }

            try
            {
                var previousCount = bindingContext.ModelState.ErrorCount;
                var model = await formatter.ReadAsync(formatterContext);

                if (bindingContext.ModelState.ErrorCount != previousCount)
                {
                    // Formatter added an error. Do not use the model it returned. As above, tell the model binding
                    // system to skip other model binders and never to fall back.
                    return new ModelBindingResult(ModelStateKey);
                }

                // Add a model state entry so we can track validation state for the model.
                bindingContext.ModelState.SetModelValue(ModelStateKey, rawValue: null, attemptedValue: null);

                var validationNode = new ModelValidationNode(ModelStateKey, bindingContext.ModelMetadata, model)
                {
                    ValidateAllProperties = true
                };

                return new ModelBindingResult(
                    model,
                    key: ModelStateKey,
                    isModelSet: true,
                    validationNode: validationNode);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(ModelStateKey, ex);

                // This model binder is the only handler for the Body binding source and it cannot run twice. Always
                // tell the model binding system to skip other model binders and never to fall back i.e. indicate a
                // fatal error.
                return new ModelBindingResult(ModelStateKey);
            }
        }
    }
}
