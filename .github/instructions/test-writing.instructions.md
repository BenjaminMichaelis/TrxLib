# TRX fixture policy for tests

When adding or updating sample `.trx` files in this repository, use **real captured TRX output**, not synthetic hand-written fixtures.

Required workflow:

1. Create temporary tests that trigger the condition you need in the TRX.
2. Run `dotnet test` with TRX output enabled to generate a real file.
3. Copy that generated TRX file into `TrxLib.Tests/SampleTrxFiles/` (renaming is fine).
4. Remove high-level personal/environment PII from the file (for example: username, machine name, local workspace paths), while preserving the TRX structure and semantics needed by tests.

Do not fabricate TRX XML by hand when a real capture is possible.
