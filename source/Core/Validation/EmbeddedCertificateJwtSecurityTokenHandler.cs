﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer3.Core.Extensions;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SecurityKey = System.IdentityModel.Tokens.SecurityKey;
using SecurityToken = System.IdentityModel.Tokens.SecurityToken;

namespace IdentityServer3.Core.Validation
{
    /// <summary>
    /// Can verify tokens that only embed "x5c" key and don't contain "x5t"
    /// </summary>
    /// <remarks>
    /// Current version implementation of <see cref="P:System.IdentityModel.Tokens.JwtHeader.SigningCredentials"/> returns NamedKeySecurityKeyIdentifierClause for "x5c" key in the token and incorrectly handles its value,
    /// which must be an array of Base64 encoded certificates according to the specification.
    /// So it is readded as X509RawDataKeyIdentifierClause, which is then correctly validated by the default JwtSecurityTokenHandler implementation
    /// </remarks>
    internal class EmbeddedCertificateJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        protected override Microsoft.IdentityModel.Tokens.SecurityKey ResolveIssuerSigningKey(string token, JwtSecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var certificate = securityToken.GetCertificateFromToken();
            if (certificate != null)
            {
                // TODO validationParameters..Add(new X509RawDataKeyIdentifierClause(certificate));
            }
            return base.ResolveIssuerSigningKey(token, securityToken, validationParameters);
        }
    }
}
