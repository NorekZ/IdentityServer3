﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */

using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Thinktecture.IdentityServer.Core.Connect;
using Thinktecture.IdentityServer.Core.Connect.Services;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.Configuration
{
    public static class AutofacConfig
    {
        public static IContainer Configure(IdentityServerCoreOptions options, Dictionary<Type, Func<object>> pluginDepencies)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (options.Factory == null) throw new InvalidOperationException("null factory");
            
            IdentityServerServiceFactory fact = options.Factory;
            fact.Validate();

            var builder = new ContainerBuilder();

            // mandatory from factory
            builder.Register(ctx => fact.AuthorizationCodeStore()).As<IAuthorizationCodeStore>();
            builder.Register(ctx => fact.CoreSettings()).As<ICoreSettings>();
            builder.Register(ctx => fact.Logger()).As<ILogger>();
            builder.Register(ctx => fact.TokenHandleStore()).As<ITokenHandleStore>();
            builder.Register(ctx => fact.UserService()).As<IUserService>();
            builder.Register(ctx => fact.ConsentService()).As<IConsentService>();

            // optional from factory
            if (fact.ClaimsProvider != null)
            {
                builder.Register(ctx => fact.ClaimsProvider()).As<IClaimsProvider>();
            }
            else
            {
                builder.RegisterType<DefaultClaimsProvider>().As<IClaimsProvider>();
            }

            if (fact.TokenService != null)
            {
                builder.Register(ctx => fact.TokenService()).As<ITokenService>();
            }
            else
            {
                builder.RegisterType<DefaultTokenService>().As<ITokenService>();
            }

            if (fact.CustomRequestValidator != null)
            {
                builder.Register(ctx => fact.CustomRequestValidator()).As<ICustomRequestValidator>();
            }
            else
            {
                builder.RegisterType<DefaultCustomRequestValidator>().As<ICustomRequestValidator>();
            }

            if (fact.AssertionGrantValidator != null)
            {
                builder.Register(ctx => fact.AssertionGrantValidator()).As<IAssertionGrantValidator>();
            }
            else
            {
                builder.RegisterType<DefaultAssertionGrantValidator>().As<IAssertionGrantValidator>();
            }

            if (fact.ExternalClaimsFilter != null)
            {
                builder.Register(ctx => fact.ExternalClaimsFilter()).As<IExternalClaimsFilter>();
            }
            else
            {
                builder.RegisterType<DefaultExternalClaimsFilter>().As<IExternalClaimsFilter>();
            }

            // validators
            builder.RegisterType<TokenRequestValidator>();
            builder.RegisterType<AuthorizeRequestValidator>();
            builder.RegisterType<ClientValidator>();
            builder.RegisterType<TokenValidator>();

            // processors
            builder.RegisterType<TokenResponseGenerator>();
            builder.RegisterType<AuthorizeResponseGenerator>();
            builder.RegisterType<AuthorizeInteractionResponseGenerator>();
            builder.RegisterType<UserInfoResponseGenerator>();

            // for authentication
            var authenticationOptions = options.AuthenticationOptions ?? new AuthenticationOptions();
            builder.RegisterInstance(authenticationOptions).AsSelf();

            // load core controller
            //builder.RegisterApiControllers(typeof(AuthorizeEndpointController).Assembly);

            // todo: right way to scan all assemblies?
            builder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());

            // load plugins
            //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var plugins = Directory.GetFiles(path, "*Plugin.dll").Select(Assembly.LoadFile).ToArray();
            //builder.RegisterApiControllers(plugins);

            if (pluginDepencies != null)
            {
                foreach (var pair in pluginDepencies)
                {
                    if (pair.Value == null)
                    {
                        builder.RegisterType(pair.Key);
                    }
                    else
                    {
                        builder.Register(ctx => pair.Value()).As(pair.Key);
                    }
                }
            }

            return builder.Build();
        }
    }
}