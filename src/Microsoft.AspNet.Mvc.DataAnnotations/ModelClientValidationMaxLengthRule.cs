// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding.Validation
{
    public class ModelClientValidationMaxLengthRule : ModelClientValidationRule
    {
        private const string MaxLengthValidationType = "maxlength";
        private const string MaxLengthValidationParameter = "max";

        public ModelClientValidationMaxLengthRule(string errorMessage, int maximumLength)
            : base(MaxLengthValidationType, errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            ValidationParameters[MaxLengthValidationParameter] = maximumLength;
        }
    }
}
