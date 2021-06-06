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

using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StitcherDownloadTool.Clients.Models.Download;
using StitcherDownloadTool.Clients.Models.Episodes;
using StitcherDownloadTool.Utilities.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StitcherDownloadTool.Clients
{
		public interface IStitcherClient
		{
				Task DownloadFile(string url, string episode_title);
				Task<DownloadModel> GetDownloadUrl(string restrictedAudioUrl);
				Task<StitcherEpisodeListResponseModel> GetEpisodes(string show);
		}

		public class StitcherClient : IStitcherClient
		{
				public StitcherClient(IFlurlClientFactory flurlClientFactory,
						ILoggerFactory loggerFactory,
						StitcherClientSettings settings)
				{
						_settings = settings ?? throw new ArgumentNullException(nameof(settings));
						_flurlClient = flurlClientFactory.Get(new Flurl.Url(settings.BaseUrl));
						_logger = loggerFactory.CreateLogger(typeof(StitcherClient).FullName);

						FlurlHttp.Configure(settings =>
						{
								settings.Redirects.Enabled = true;
								settings.Redirects.ForwardAuthorizationHeader = true;
								settings.Redirects.MaxAutoRedirects = 5;
						});
				}

				public async Task<StitcherEpisodeListResponseModel> GetEpisodes(string show)
				{
						_logger.LogInformation($"{GetType()}: Downloading episode list.");

						try
						{
								var episodes = await _flurlClient
								.Request(_settings.GetEpisodeListPath.Format(new { show }))
								.GetAsync()
								.ReceiveJson<StitcherEpisodeListResponseModel>();

								_logger.LogInformation($"{GetType()}: {episodes.Data.Episodes.Count()} episodes fetched in episode list.");

								return episodes;
						}

						catch (FlurlHttpException ex)
						{
								throw new Exception($"Failed to get episode list for show {show}: {ex.Message}");
						}
				}

				public async Task<DownloadModel> GetDownloadUrl(string restrictedAudioUrl)
				{
						_logger.LogInformation($"{GetType()}: Fetching download URL from restricted URL: GET {restrictedAudioUrl}");

						try
						{
								var httpClient = new HttpClient();

								var requestUri = $"{restrictedAudioUrl}?client=web";

								var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

								request.Headers.Add("Authorization", $"Bearer {_settings.Bearer}");

								var response = await httpClient.SendAsync(request);

								var content = await response.Content.ReadAsStringAsync();

								var downloadModel = JsonConvert.DeserializeObject<DownloadModel>(content);

								_logger.LogInformation($"{GetType()}: Successfully fetched download URL: {downloadModel.Url}");

								return downloadModel;
						}

						catch (Exception ex)
						{
								throw new Exception($"Failed to get download URL for restricted URL {restrictedAudioUrl}: {ex.Message}");
						}
				}

				public async Task DownloadFile(string url, string episode_title)
				{
						_logger.LogInformation($"{GetType()}: Download started for URL: {url}");

						var client = new HttpClient();

						try
						{
								using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))

								using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
								{
										string fileToWriteTo = $"{_settings.DownloadLocation}{episode_title}.mp3";
										using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
										{
												await streamToReadFrom.CopyToAsync(streamToWriteTo);
										}
								}
						}

						catch (Exception ex)
						{
								throw new Exception($"Failed to download file from URL {url}: {ex.Message}");
						}

						_logger.LogInformation($"{GetType()}: Successfully download file from URL: {url}");
				}

				private readonly IFlurlClient _flurlClient;
				private readonly ILogger _logger;
				private readonly StitcherClientSettings _settings;

		}
}
