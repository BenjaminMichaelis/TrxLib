---
name: trx-fixture-policy
description: Use this skill when adding or updating sample .trx files in TrxLib.Tests/SampleTrxFiles. It describes the required workflow for capturing real TRX output from dotnet test rather than hand-writing synthetic XML.
---

# TRX fixture policy for tests

When adding or updating sample `.trx` files in this repository, use **real captured TRX output**, not synthetic hand-written fixtures.

Required workflow:

1. Create temporary tests that trigger the condition you need in the TRX.
2. Run `dotnet test` with TRX output enabled to generate a real file.
3. Copy that generated TRX file into `TrxLib.Tests/SampleTrxFiles/` (renaming is fine).
4. Remove high-level personal/environment PII from the file (for example: username, machine name, local workspace paths), while preserving the TRX structure and semantics needed by tests.

Do not fabricate TRX XML by hand when a real capture is possible.
