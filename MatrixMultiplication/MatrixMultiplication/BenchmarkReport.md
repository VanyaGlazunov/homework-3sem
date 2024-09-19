```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4780/22H2/2022Update)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.303
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2


```
| Method       | N    | Mean          | Error       | StdDev      | Median        |
|------------- |----- |--------------:|------------:|------------:|--------------:|
| **Parallel**     | **100**  |      **1.577 ms** |   **0.0165 ms** |   **0.0154 ms** |      **1.575 ms** |
| Non-Parallel | 100  |      2.026 ms |   0.0337 ms |   0.0282 ms |      2.032 ms |
| **Parallel**     | **200**  |      **6.838 ms** |   **0.0337 ms** |   **0.0315 ms** |      **6.845 ms** |
| Non-Parallel | 200  |     15.989 ms |   0.3164 ms |   0.2642 ms |     16.065 ms |
| **Parallel**     | **400**  |     **43.675 ms** |   **0.6590 ms** |   **0.6164 ms** |     **43.350 ms** |
| Non-Parallel | 400  |    132.863 ms |   2.5808 ms |   3.1695 ms |    134.838 ms |
| **Parallel**     | **800**  |    **346.507 ms** |   **2.9934 ms** |   **2.4996 ms** |    **345.343 ms** |
| Non-Parallel | 800  |  1,251.896 ms |  14.0053 ms |  12.4153 ms |  1,248.523 ms |
| **Parallel**     | **1600** |  **3,327.885 ms** |  **27.6322 ms** |  **23.0741 ms** |  **3,331.963 ms** |
| Non-Parallel | 1600 | 10,839.934 ms | 208.7810 ms | 256.4018 ms | 10,816.465 ms |
