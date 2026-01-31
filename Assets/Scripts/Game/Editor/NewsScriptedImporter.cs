using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "news")]
public sealed class NewsScriptedImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Read the source .news file
        string text;
        try
        {
            text = File.ReadAllText(ctx.assetPath);
        }
        catch (IOException e)
        {
            ctx.LogImportError($"Could not read .news file: {e.Message}");
            return;
        }

        // Create the main asset (the .news file itself will import as a News object)
        var news = ScriptableObject.CreateInstance<News>();
        news.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
        news.items.Clear();

        // Parse and populate
        news.items.AddRange(Parse(text, ctx));

        // Register as the imported asset
        ctx.AddObjectToAsset("News", news);
        ctx.SetMainObject(news);
    }

    private static IEnumerable<News.Item> Parse(string text, AssetImportContext ctx)
    {
        // Split lines, trimming whitespace; ignore empty lines
        var lines = new List<string>();
        using (var sr = new StringReader(text))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }
        }

        if (lines.Count == 0)
        {
            ctx.LogImportWarning("File is empty (no news items).");
            yield break;
        }

        if (lines.Count % 2 != 0)
        {
            ctx.LogImportWarning(
                $"Odd number of non-empty lines ({lines.Count}). Last line will be ignored."
            );
        }

        for (int i = 0; i + 1 < lines.Count; i += 2)
        {
            string boolLine = lines[i];
            string headline = lines[i + 1];

            if (!bool.TryParse(boolLine, out bool side))
            {
                ctx.LogImportWarning(
                    $"Entry {i / 2} skipped: expected 'true' or 'false' but got '{boolLine}'."
                );
                continue;
            }

            if (string.IsNullOrWhiteSpace(headline))
            {
                ctx.LogImportWarning($"Entry {i / 2} skipped: empty headline.");
                continue;
            }

            yield return new News.Item
            {
                causeFaction = side,
                newsItem = headline
            };
        }
    }
}
