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

using System.Collections.Generic;
using System.Linq;

namespace StitcherDownloadTool.Utilities.Extensions
{
		public static class CommonExtensions
		{
				public static string Format(this string formatString, object parameters)
				{
						var pairs = parameters.GetKeyValuePairs();

						foreach (var pair in pairs)
						{
								formatString = formatString.Replace("{" + pair.Key + "}", pair.Value.ToString());
						}

						return formatString;
				}

				public static Dictionary<string, object> GetKeyValuePairs(this object obj)
				{
						var pairs = obj
								.GetType()
								.GetProperties()
								.ToDictionary(x => x.Name, y => y.GetValue(obj));

						return pairs;					
				}
		}
}
