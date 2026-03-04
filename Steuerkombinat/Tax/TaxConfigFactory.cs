using System;
using System.Collections.Generic;

namespace Steuerkombinat.Tax;

public static class TaxConfigFactory {
    private static readonly Dictionary<int, TaxYearConfig> _configs = new() {
        { 2025, Create2025Config() },
        { 2026, Create2026Config() }
    };

    public static TaxYearConfig GetConfig(int year) {
        if (_configs.TryGetValue(year, out var config)) {
            return config;
        }
        // find the closest year instead.
        int bestDist = year;
        TaxYearConfig bestCfg = null;
        foreach (var kvp in _configs) {
            int dist = Math.Abs(kvp.Key - year);
            if (dist < bestDist) {
                bestDist = dist;
                bestCfg = kvp.Value;
            }
        }

        if (bestCfg == null) {
            throw new ArgumentException($"No tax configuration available for year {year}");
        }

        return bestCfg;
    }

    public static bool HasConfig(int year) => _configs.ContainsKey(year);
    
    private static TaxYearConfig Create2025Config() {
        // Based on §32a EStG for 2025
        return new TaxYearConfig {
            Year = 2025,
            Zones = new List<TaxZone> {
                // Zone 0: Tax-free (Grundfreibetrag) - up to 12,096€
                new() {
                    AmountStart = 0,
                    AmountEnd = 12096,
                    Type = TaxZoneType.TaxFree
                },
                // Zone 1: First progressive zone
                // Progressive formula: (coefficient_a * y + 1,400) * y
                // where y = (income_in_zone) / 10,000
                new() {
                    AmountStart = 12097,
                    AmountEnd = 17443,
                    Type = TaxZoneType.ProgressiveQuadratic,
                    CoefficientA = 932.3m,
                    CoefficientB = 1400m
                },
                // Zone 2: Second progressive zone
                // Progressive formula: (coefficient_a * z + 2,397) * z
                // where z = (income_in_zone) / 10,000
                new() {
                    AmountStart = 17444,
                    AmountEnd = 68480,
                    Type = TaxZoneType.ProgressiveQuadratic,
                    CoefficientA = 176.64m,
                    CoefficientB = 2397m
                },
                // Zone 3: Linear zone at 42%
                new() {
                    AmountStart = 68481,
                    AmountEnd = 277825,
                    Type = TaxZoneType.Linear,
                    Rate = 0.42m
                },
                // Zone 4: Reichensteuer, lol 45%
                new() {
                    AmountStart = 277826,
                    AmountEnd = decimal.MaxValue,
                    Type = TaxZoneType.Linear,
                    Rate = 0.45m
                }
            }
        };
    }
    
    private static TaxYearConfig Create2026Config() {
        // Based on §32a EStG for 2026
        return new TaxYearConfig {
            Year = 2025,
            Zones = new List<TaxZone> {
                // Zone 0: Tax-free (Grundfreibetrag) - up to 12,096€
                new() {
                    AmountStart = 0,
                    AmountEnd = 12348,
                    Type = TaxZoneType.TaxFree
                },
                // Zone 1: First progressive zone
                // Progressive formula: (coefficient_a * y + 1,400) * y
                // where y = (income_in_zone) / 10,000
                new() {
                    AmountStart = 12349,
                    AmountEnd = 17799,
                    Type = TaxZoneType.ProgressiveQuadratic,
                    CoefficientA = 914.51m,
                    CoefficientB = 1400m
                },
                // Zone 2: Second progressive zone
                // Progressive formula: (coefficient_a * z + 2,397) * z
                // where z = (income_in_zone) / 10,000
                new() {
                    AmountStart = 17444,
                    AmountEnd = 68480,
                    Type = TaxZoneType.ProgressiveQuadratic,
                    CoefficientA = 173.1m,
                    CoefficientB = 2397m
                },
                // Zone 3: Linear zone at 42%
                new() {
                    AmountStart = 68481,
                    AmountEnd = 277825,
                    Type = TaxZoneType.Linear,
                    Rate = 0.42m
                },
                // Zone 4: Reichensteuer, lol 45%
                new() {
                    AmountStart = 277826,
                    AmountEnd = decimal.MaxValue,
                    Type = TaxZoneType.Linear,
                    Rate = 0.45m
                }
            }
        };
    }
}
