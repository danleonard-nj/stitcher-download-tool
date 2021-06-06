/* Copyright (C) 2021 Dan Leonard
 * 
 * This is free software: you can redistribute it and/or modify it under 
 * the terms of the GNU General Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) 
 * any later version.
 * 
 * This software is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
 * for more details.
 */

using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StitcherDownloadTool.Clients;
using StitcherDownloadTool.Components.Stitcher;

namespace StitcherDownloadTool.Configuration
{
		public static class ConfigureServices
		{
				public static IServiceCollection ConfigureServiceCollection()
				{
						var serviceCollection = new ServiceCollection();

						var configuration = new ConfigurationBuilder()
								.AddJsonFile("appsettings.json")
								.Build();

						serviceCollection.AddTransient<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
						serviceCollection.AddTransient<IStitcherClient, StitcherClient>();
						serviceCollection.AddTransient<IStitcherComponent, StitcherComponent>();
						serviceCollection.AddTransient<IApplication, Application>();

						serviceCollection.RegisterConfiguration<StitcherClientSettings>(configuration);

						serviceCollection.AddLogging(
								configure => configure
										.AddConsole()
										.AddDebug());

						return serviceCollection;
				}
		}
}