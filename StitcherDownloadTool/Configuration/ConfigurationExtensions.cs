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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StitcherDownloadTool.Configuration
{
		public static class ConfigurationExtensions
		{
				public static void RegisterConfiguration<T>(this IServiceCollection serviceDescriptors, IConfiguration configuration) where T : class
				{
						var instance = Activator.CreateInstance<T>();

						configuration.GetSection(typeof(T).Name).Bind(instance);

						serviceDescriptors.AddSingleton(instance);
				}
		}
}
