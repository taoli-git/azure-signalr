﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR;

namespace ChatSample.CoreApp3
{
    public class Chat : Hub
    {
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
            Console.WriteLine("Broadcasting...");
        }

        public void Echo(string name, string message)
        {
            Clients.User(name).SendAsync("echo", name, $"{message}");
            //Clients.Client(Context.ConnectionId).SendAsync("echo", name, $"{message} (echo from server, Client IP: {Context.GetHttpContext().Connection.RemoteIpAddress})");
            Console.WriteLine("[Application Layer]\tReceived Message: " + message + " from user " + name);
            Console.WriteLine();
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("[Application Layer]\t" + $"{Context.ConnectionId} connected.");

            var feature = Context.GetHttpContext().Features.Get<IConnectionMigrationFeature>();
            if (feature != null)
            {
                Console.WriteLine($"[{feature.MigrateTo}] {Context.ConnectionId} is migrated from {feature.MigrateFrom}.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine("[Application Layer]\t" + $"{Context.ConnectionId} disconnected.");

            var feature = Context.GetHttpContext().Features.Get<IConnectionMigrationFeature>();
            if (feature != null)
            {
                Console.WriteLine($"[{feature.MigrateFrom}] {Context.ConnectionId} will be migrated to {feature.MigrateTo}.");
            }

            await base.OnDisconnectedAsync(e);
        }
    }
}