﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HA4IoT.TraceViewer
{
    public class TraceItemsParser
    {
        public IList<TraceItem> Parse(string source)
        {
            JObject package = JObject.Parse(source);
            string type = package.Property("Type").Value.ToString();
            int version = (int)package.Property("Version").Value;

            var traceItems = new List<TraceItem>();
            foreach (var notification in package.Property("TraceItems").Value)
            {
                var item = notification.ToObject<JObject>();

                long id = long.Parse(item.Property("Id").Value.ToString());
                DateTime timestamp = DateTime.Parse(item.Property("Timestamp").Value.ToString());
                int threadId = int.Parse(item.Property("ThreadId").Value.ToString());
                string severity = item.Property("Severity").Value.ToString();
                string message = item.Property("Message").Value.ToString();

                var typeValue = (TraceItemSeverity)Enum.Parse(typeof(TraceItemSeverity), severity);
                traceItems.Add(new TraceItem(id, timestamp, threadId, typeValue, message));
            }

            return traceItems;
        }
    }
}
