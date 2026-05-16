using TrxLib;

// Write a minimal TRX to a temp file and actually parse it so the full
// XDocument path executes in the native binary.
const string minimalTrx =
    "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
    "<TestRun id=\"aot-smoke-run\" name=\"AOT Smoke Test\" " +
    "xmlns=\"http://microsoft.com/schemas/VisualStudio/TeamTest/2010\">" +
    "<Results /><TestDefinitions />" +
    "</TestRun>";

var tempFile = Path.Combine(Path.GetTempPath(), $"aot-smoke-{Guid.NewGuid()}.trx");
try
{
    File.WriteAllText(tempFile, minimalTrx, System.Text.Encoding.UTF8);
    var result = TrxParser.Parse(new FileInfo(tempFile));
    if (result.TestRunName != "AOT Smoke Test")
        throw new InvalidOperationException($"Unexpected TestRunName: '{result.TestRunName}'");
    Console.WriteLine("TrxLib AOT validation passed.");
}
finally
{
    if (File.Exists(tempFile))
        File.Delete(tempFile);
}
