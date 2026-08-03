using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DreamPoeBot.Loki.Game.Objects;
using Newtonsoft.Json;

namespace AllflameBot
{
    internal sealed class AllflameUniqueArtResolution
    {
        public string RenderArt { get; set; }
        public List<string> Candidates { get; set; }

        public bool IsMapped
        {
            get { return Candidates != null && Candidates.Count > 0; }
        }
    }

    internal static class AllflameUniqueResolver
    {
        private const string MappingFileName =
            "uniqueArtMapping.default.json";
        private static readonly object Sync = new object();
        private static Dictionary<string, List<string>> _mapping;
        private static bool _loadAttempted;

        public static AllflameUniqueArtResolution Resolve(Item item)
        {
            var result = new AllflameUniqueArtResolution
            {
                RenderArt = item == null
                    ? String.Empty
                    : (item.RenderArt ?? String.Empty).Trim(),
                Candidates = new List<string>()
            };
            if (item == null ||
                item.IsIdentified ||
                !String.Equals(
                    item.Rarity.ToString(),
                    "Unique",
                    StringComparison.OrdinalIgnoreCase) ||
                String.IsNullOrWhiteSpace(result.RenderArt))
            {
                return result;
            }

            EnsureLoaded();
            List<string> candidates;
            lock (Sync)
            {
                if (_mapping == null ||
                    !_mapping.TryGetValue(
                        NormalizeArtPath(result.RenderArt),
                        out candidates))
                {
                    return result;
                }
                result.Candidates = candidates.ToList();
            }
            return result;
        }

        private static void EnsureLoaded()
        {
            lock (Sync)
            {
                if (_loadAttempted)
                {
                    return;
                }
                _loadAttempted = true;
                string path = FindMappingPath();
                if (String.IsNullOrWhiteSpace(path))
                {
                    AllflameLog.Info(
                        "UNIQUE_ART_MAPPING_LOAD" +
                        "|Result=Missing" +
                        "|File=" + MappingFileName);
                    return;
                }

                try
                {
                    Dictionary<string, List<string>> source =
                        JsonConvert.DeserializeObject<
                            Dictionary<string, List<string>>>(
                                File.ReadAllText(path));
                    _mapping = (source ??
                        new Dictionary<string, List<string>>())
                        .Where(pair =>
                            !String.IsNullOrWhiteSpace(pair.Key) &&
                            pair.Value != null &&
                            pair.Value.Any(name =>
                                !String.IsNullOrWhiteSpace(name)))
                        .ToDictionary(
                            pair => NormalizeArtPath(pair.Key),
                            pair => pair.Value
                                .Where(name =>
                                    !String.IsNullOrWhiteSpace(name))
                                .Select(name => name.Trim())
                                .Distinct(
                                    StringComparer.OrdinalIgnoreCase)
                                .ToList(),
                            StringComparer.OrdinalIgnoreCase);
                    AllflameLog.Info(
                        "UNIQUE_ART_MAPPING_LOAD" +
                        "|Result=Success" +
                        "|Entries=" + _mapping.Count +
                        "|Path=" + path);
                }
                catch (Exception ex)
                {
                    _mapping = null;
                    AllflameLog.Info(
                        "UNIQUE_ART_MAPPING_LOAD" +
                        "|Result=Failed" +
                        "|Type=" + ex.GetType().Name +
                        "|Message=" +
                        (ex.Message ?? String.Empty).Replace("|", "/"));
                }
            }
        }

        private static string FindMappingPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string assemblyDirectory = Path.GetDirectoryName(
                typeof(AllflameUniqueResolver).Assembly.Location) ??
                String.Empty;
            string[] candidates =
            {
                Path.Combine(
                    assemblyDirectory,
                    "Data",
                    MappingFileName),
                Path.Combine(
                    baseDirectory,
                    "Data",
                    MappingFileName),
                Path.Combine(
                    baseDirectory,
                    "3rdParty",
                    "AllflameBot",
                    "Data",
                    MappingFileName)
            };
            return candidates.FirstOrDefault(File.Exists);
        }

        private static string NormalizeArtPath(string path)
        {
            return (path ?? String.Empty)
                .Trim()
                .Replace('\\', '/');
        }
    }
}
