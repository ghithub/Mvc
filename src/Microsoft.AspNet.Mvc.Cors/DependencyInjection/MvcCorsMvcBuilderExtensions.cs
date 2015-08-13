// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.Framework.DependencyInjection.Extensions;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.DependencyInjection
{
    public static class MvcCorsMvcBuilderExtensions
    {
        public static IMvcBuilder AddCors(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddCorsServices(builder.Services);
            return builder;
        }

        public static IMvcBuilder AddCors(
            this IMvcBuilder builder,
            Action<CorsOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            AddCorsServices(builder.Services);

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }

        public static IMvcBuilder ConfigureCors(
            this IMvcBuilder builder,
            Action<CorsOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.Services.Configure(setupAction);
            return builder;
        }

        // Internal for testing.
        internal static void AddCorsServices(IServiceCollection services)
        {
            services.AddCors();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, CorsApplicationModelProvider>());
            services.TryAddTransient<CorsAuthorizationFilter, CorsAuthorizationFilter>();
        }
    }
}
