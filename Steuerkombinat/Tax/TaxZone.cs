using System;

namespace Steuerkombinat.Tax;

public enum TaxZoneType {
    TaxFree,
    ProgressiveQuadratic,
    Linear
}

public class TaxZone {
    public decimal AmountStart { get; init; }
    public decimal AmountEnd { get; init; }
    public TaxZoneType Type { get; init; }

    // For Linear zones
    public decimal Rate { get; init; }

    // For quadratic progressive zones: Tax = (A * y + B) * y
    // where y = (income_in_zone - 1) / 10000
    // This calculates an effective rate that varies across the zone
    public decimal CoefficientA { get; init; }
    public decimal CoefficientB { get; init; }

    /// <summary>
    /// Calculates tax for the portion of income that falls within this zone
    /// </summary>
    public decimal CalculateTaxForZone(decimal totalIncome) {
        if (totalIncome <= AmountStart) {
            return 0;
        }

        // How much income falls in this zone
        // removes 1 from start because that is the end from previous zone (which is what the official calculation does)
        var incomeInZone = Math.Min(totalIncome, AmountEnd) - (AmountStart - 1);
        if (incomeInZone <= 0) {
            return 0;
        }

        return Type switch {
            TaxZoneType.TaxFree => 0,
            TaxZoneType.Linear => CalculateLinearTax(incomeInZone),
            TaxZoneType.ProgressiveQuadratic => CalculateProgressiveTax(incomeInZone),
            _ => 0
        };
    }

    private decimal CalculateLinearTax(decimal incomeInZone) {
        return incomeInZone * Rate;
    }

    private decimal CalculateProgressiveTax(decimal incomeInZone) {
        var y = (incomeInZone - 1) / 10000m;
        // Tax formula for progressive zones: (A * y + B) * y
        return (CoefficientA * y + CoefficientB) * y;
    }
}
