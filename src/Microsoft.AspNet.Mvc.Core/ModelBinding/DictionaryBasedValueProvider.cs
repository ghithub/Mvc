// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    /// <summary>
    /// An <see cref="IValueProvider"/> adapter for data stored in an
    /// <see cref="IDictionary{string, object}"/>.
    /// </summary>
    public class DictionaryBasedValueProvider : BindingSourceValueProvider
    {
        private readonly IDictionary<string, object> _values;
        private PrefixContainer _prefixContainer;

        /// <summary>
        /// Creates a new <see cref="DictionaryBasedValueProvider"/>.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource"/> of the data.</param>
        /// <param name="values">The values.</param>
        public DictionaryBasedValueProvider(
            BindingSource bindingSource,
            IDictionary<string, object> values)
            : base(bindingSource)
        {
            if (bindingSource == null)
            {
                throw new ArgumentNullException(nameof(bindingSource));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            _values = values;
        }

        /// <inheritdoc />
        public override Task<bool> ContainsPrefixAsync(string key)
        {
            var prefixContainer = GetOrCreatePrefixContainer();
            return Task.FromResult(prefixContainer.ContainsPrefix(key));
        }

        private PrefixContainer GetOrCreatePrefixContainer()
        {
            if (_prefixContainer == null)
            {
                _prefixContainer = new PrefixContainer(_values.Keys);
            }

            return _prefixContainer;
        }

        /// <inheritdoc />
        public override Task<ValueProviderResult> GetValueAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            object value;
            ValueProviderResult result;
            if (_values.TryGetValue(key, out value))
            {
                var attemptedValue = value != null ? value.ToString() : null;
                result = new ValueProviderResult(value, attemptedValue, CultureInfo.InvariantCulture);
            }
            else
            {
                result = null;
            }

            return Task.FromResult(result);
        }
    }
}
