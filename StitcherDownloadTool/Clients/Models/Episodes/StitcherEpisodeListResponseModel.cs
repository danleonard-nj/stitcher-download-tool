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

using Newtonsoft.Json;

namespace StitcherDownloadTool.Clients.Models.Episodes
{
		public class StitcherEpisodeListResponseModel
		{
				[JsonProperty(PropertyName = "data")]
				public StitcherEpisodeListModel Data { get; set; }
		}
}
