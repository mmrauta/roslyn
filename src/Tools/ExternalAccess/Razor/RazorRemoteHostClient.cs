﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Experiments;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Remote;

namespace Microsoft.CodeAnalysis.ExternalAccess.Razor
{
    internal sealed class RazorRemoteHostClient
    {
        private readonly RemoteHostClient _client;

        internal RazorRemoteHostClient(RemoteHostClient client)
        {
            _client = client;
        }

        public static async Task<RazorRemoteHostClient?> CreateAsync(Workspace workspace, CancellationToken cancellationToken = default)
        {
            var clientFactory = workspace.Services.GetRequiredService<IRemoteHostClientProvider>();
            var client = await clientFactory.TryGetRemoteHostClientAsync(cancellationToken).ConfigureAwait(false);
            return client == null ? null : new RazorRemoteHostClient(client);
        }

        public Task<Optional<T>> TryRunRemoteAsync<T>(string targetName, Solution? solution, IReadOnlyList<object?> arguments, CancellationToken cancellationToken)
            => _client.TryRunRemoteAsync<T>(WellKnownServiceHubService.Razor, targetName, solution, arguments, callbackTarget: null, cancellationToken);
    }
}
