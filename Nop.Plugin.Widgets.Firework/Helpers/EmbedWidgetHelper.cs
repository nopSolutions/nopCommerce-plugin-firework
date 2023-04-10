﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Plugin.Widgets.Firework.Helpers
{
    public class EmbedWidgetHelper
    {
        public static List<string> GetCustomWidgetZones()
        {
            var zones = new List<string>();
            try
            {
                var list = GetCustomWidgetZoneNameValues();
                if (list != null && list.Any())
                    zones.AddRange(list.Select(x => x.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
            }
            catch (Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>().ErrorAsync(ex.Message, ex).Wait();
            }
            return zones;
        }

        public static List<WidgetZoneModel> GetCustomWidgetZoneNameValues()
        {
            var zones = new List<WidgetZoneModel>();
            try
            {
                var nopFileProvider = EngineContext.Current.Resolve<INopFileProvider>();
                var filePath = nopFileProvider.Combine(nopFileProvider.MapPath("~/Plugins/Widgets.Firework"), "widgetZones.json");

                if (nopFileProvider.FileExists(filePath))
                {
                    var jsonstr = nopFileProvider.ReadAllText(filePath, Encoding.UTF8);
                    var list = JsonConvert.DeserializeObject<List<WidgetZoneModel>>(jsonstr);
                    if (list != null && list.Any())
                        return list;
                }
            }
            catch (Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>().ErrorAsync(ex.Message, ex).Wait();
            }
            return zones;
        }

        public static bool TryGetWidgetZoneId(string widgetZone, out int widgetZoneId)
        {
            widgetZoneId = -1;
            var zones = GetCustomWidgetZoneNameValues();
            if (zones != null && zones.Any(x => x.Name.Equals(widgetZone)))
            {
                widgetZoneId = zones.FirstOrDefault(x => x.Name.Equals(widgetZone)).Value;
                return true;
            }
            return false;
        }

        public static string GetCustomWidgetZone(int widgetZoneId)
        {
            var zones = GetCustomWidgetZoneNameValues();
            if (zones != null && zones.Any(x => x.Value == widgetZoneId))
            {
                return zones.FirstOrDefault(x => x.Value == widgetZoneId).Name;
            }
            return null;
        }

        public static IList<SelectListItem> GetCustomWidgetZoneSelectList()
        {
            var list = new List<SelectListItem>();
            var zones = GetCustomWidgetZoneNameValues();
            if (zones != null && zones.Any())
            {
                foreach (var item in zones)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.Value.ToString(),
                        Text = item.Name
                    });
                }
            }
            return list;
        }
    }
}