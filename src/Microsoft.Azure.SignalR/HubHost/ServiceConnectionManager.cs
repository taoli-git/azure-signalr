﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Protocol;

namespace Microsoft.Azure.SignalR
{
    internal class ServiceConnectionManager<THub> : IServiceConnectionManager<THub> where THub : Hub
    {
        private readonly List<ServiceConnection> _serviceConnections = new List<ServiceConnection>();

        public void AddServiceConnection(ServiceConnection serviceConnection)
        {
            _serviceConnections.Add(serviceConnection);
        }

        public async Task StartAsync()
        {
            var tasks = _serviceConnections.Select(c => c.StartAsync());
            await Task.WhenAll(tasks);
        }

        public async Task WriteAsync(ServiceMessage serviceMessage)
        {
            if (_serviceConnections.Count == 0)
            {
                throw new InvalidOperationException("Azure SignalR Service is not connected yet, please try again later.");
            }

            var index = StaticRandom.Next(_serviceConnections.Count);
            await _serviceConnections[index].WriteAsync(serviceMessage);
        }

        public async Task WriteAsync(string partitionKey, ServiceMessage serviceMessage)
        {
            if (_serviceConnections.Count == 0)
            {
                throw new InvalidOperationException("Azure SignalR Service is not connected yet, please try again later.");
            }

            // If we hit this check, it is a code bug.
            if (string.IsNullOrEmpty(partitionKey))
            {
                throw new ArgumentNullException(nameof(partitionKey));
            }

            var index = (partitionKey.GetHashCode() & int.MaxValue) % _serviceConnections.Count;
            await _serviceConnections[index].WriteAsync(serviceMessage);
        }
    }
}
