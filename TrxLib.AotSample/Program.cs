using TrxLib;

var sampleRoot = Path.Combine(AppContext.BaseDirectory, "SampleTrxFiles");
if (!Directory.Exists(sampleRoot))
    throw new DirectoryNotFoundException($"Sample TRX directory not found: {sampleRoot}");

var sampleFiles = Directory
    .EnumerateFiles(sampleRoot, "*.trx", SearchOption.AllDirectories)
    .OrderBy(path => path, StringComparer.Ordinal)
    .ToArray();

if (sampleFiles.Length == 0)
    throw new InvalidOperationException($"No TRX sample files found under: {sampleRoot}");

foreach (var file in sampleFiles)
{
    _ = TrxParser.Parse(new FileInfo(file));
}

Console.WriteLine($"TrxLib AOT validation passed. Parsed {sampleFiles.Length} sample files.");
