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

using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace IdentityServer3.Core.Extensions
{
    internal static class JwtSecurityTokenExtensions
    {
        public static X509Certificate2 GetCertificateFromToken(this JwtSecurityToken securityToken)
        {
            object values;
            if (securityToken.Header.TryGetValue(JwtHeaderParameterNames.X5c, out values))
            {
                var jArray = values as JArray;
                if (jArray != null)
                {
                    var objects = jArray.ToList<object>();
                    var rawCertificate = objects.Select(o => o.ToString()).FirstOrDefault();
                    if (rawCertificate != null)
                    {
                        return new X509Certificate2(Convert.FromBase64String(rawCertificate));
                    }
                }
            }
            return null;
        }
    }
}
