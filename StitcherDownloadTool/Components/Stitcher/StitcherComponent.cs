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

using StitcherDownloadTool.Clients;
using StitcherDownloadTool.Clients.Models.Episodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StitcherDownloadTool.Components.Stitcher
{
		public interface IStitcherComponent
		{
				Task DownloadEpisode(StitcherEpisodeModel episode);
				Task<IEnumerable<StitcherEpisodeModel>> GetEpisodes(string show);
		}

		public class StitcherComponent : IStitcherComponent
		{
				public StitcherComponent(IStitcherClient stitcherClient)
				{
						_stitcherClient = stitcherClient;
				}

				public async Task<IEnumerable<StitcherEpisodeModel>> GetEpisodes(string show)
				{
						var response = await _stitcherClient.GetEpisodes(show);

						return response.Data.Episodes;
				}

				public async Task DownloadEpisode(StitcherEpisodeModel episode)
				{
						Console.WriteLine($"{GetType()}: Episode {episode.EpisodeId}: Title {episode.Title}: starting download.");

						var downloadUrl = await _stitcherClient.GetDownloadUrl(episode.AudioUrlRestricted);

						await _stitcherClient.DownloadFile(downloadUrl.Url, episode.Title);

						Console.WriteLine($"{GetType()}: Episode {episode.EpisodeId}: Title {episode.Title}: downloaded successfully.");
				}

				private readonly IStitcherClient _stitcherClient;
		}
}
