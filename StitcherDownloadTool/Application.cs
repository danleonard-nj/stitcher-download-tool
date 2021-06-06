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

using Dapper;
using Microsoft.Extensions.Logging;
using StitcherDownloadTool.Components.Stitcher;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StitcherDownloadTool
{
		public interface IApplication
		{
				Task Run();
		}

		public class Application : IApplication
		{
				public Application(IStitcherComponent stitcherComponent,
						ILoggerFactory loggerFactory)
				{
						_stitcherComponent = stitcherComponent;
						_logger = loggerFactory.CreateLogger(typeof(Application).Name);
				}

				public async Task DownloadSingle()
				{
						var episodes = await _stitcherComponent.GetEpisodes("comedy-bang-bang-the-podcast");

						await _stitcherComponent.DownloadEpisode(episodes.First());
				}

				public async Task DownloadConcurrent()
				{
						List<string> episodesToDownload;
										
						var allEpisodes = await _stitcherComponent.GetEpisodes("comedy-bang-bang-the-podcast");

						Console.WriteLine("Initiating download...");

						using (var concurrency = new SemaphoreSlim(8, 10))
						{
								var tasks = episodes.Select(async episode =>
								{
										await concurrency.WaitAsync();

										try
										{
												await _stitcherComponent.DownloadEpisode(episode);
										}

										catch (Exception ex)
										{
												_logger.LogError($"{GetType()}: Episode {episode.EpisodeId}: Failed to download file from URL");
												_logger.LogError($"{GetType()}: Episode {episode.EpisodeId}: Exception Type: {ex.GetType().Name}: Message: {ex.Message}");
										}

										finally
										{
												concurrency.Release();
										}
								});

								await Task.WhenAll(tasks);
						}
				}

				public async Task Run()
				{
						await DownloadConcurrent();
				}

				private readonly IStitcherComponent _stitcherComponent;
				private readonly ILogger _logger;		
		}
}
