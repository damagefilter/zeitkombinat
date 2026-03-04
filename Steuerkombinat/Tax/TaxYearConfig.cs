using System;
using System.Collections.Generic;
using System.Linq;

namespace Steuerkombinat.Tax;

public class TaxYearConfig {
    public int Year { get; init; }
    public List<TaxZone> Zones { get; init; } = new();

    public decimal CalculateTax(decimal income) {
        if (income <= 0) return 0;

        // Round down to full euro as per official formula
        var roundedIncome = Math.Floor(income);

        // Calculate tax by running through all zones and summing up
        decimal totalTax = 0;

        foreach (var zone in Zones.OrderBy(z => z.AmountStart)) {
            // Only process zones that this income touches
            if (roundedIncome <= zone.AmountStart) {
                // break would probably be in order because this is sorted but eh.
                continue;
            }

            var zoneTax = zone.CalculateTaxForZone(roundedIncome);
            totalTax += zoneTax;
        }

        return totalTax;
    }
}
