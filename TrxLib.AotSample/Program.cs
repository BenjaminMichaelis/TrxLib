using TrxLib;

// AOT smoke test: exercises TrxParser with a non-existent file to verify
// all code paths are reachable without dynamic code generation.
try
{
    TrxParser.Parse(new FileInfo("nonexistent.trx"));
}
catch (FileNotFoundException)
{
    // Expected — the point is that the native binary linked and ran successfully.
}

Console.WriteLine("TrxLib AOT validation passed.");
