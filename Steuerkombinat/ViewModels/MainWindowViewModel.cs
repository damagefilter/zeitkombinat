using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Steuerkombinat.Data;
using Steuerkombinat.Models;
using Steuerkombinat.Tax;

namespace Steuerkombinat.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Kv))]
    [NotifyPropertyChangedFor(nameof(Eks))]
    [NotifyPropertyChangedFor(nameof(TotalAfterDeductions))]
    private decimal invoiceAmount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Kv))]
    [NotifyPropertyChangedFor(nameof(Eks))]
    [NotifyPropertyChangedFor(nameof(TotalAfterDeductions))]
    private string invoiceNumber = string.Empty;

    [ObservableProperty]
    private decimal currentKv;

    public ObservableCollection<Invoice> Invoices { get; } = [];

    public decimal Kv => (InvoiceAmount) * 0.19m;

    public decimal Eks {
        get {
            var taxableIncome = (InvoiceAmount * 12) - (Kv * 12);
            var taxConfig = TaxConfigFactory.GetConfig(DateTime.Now.Year);
            var tax = taxConfig.CalculateTax(taxableIncome);
            return tax / 12;
        }
    }

    public decimal TotalAfterDeductions => ((InvoiceAmount) - Kv - Eks);

    public decimal YearlyTotal => Invoices
        .Where(i => i.Date.Year == DateTime.Now.Year)
        .Sum(i => i.Amount);

    public decimal YearlyKv => YearlyTotal * 0.19m;

    public decimal YearlyEks {
        get {
            var taxableIncome = YearlyTotal - YearlyKv;
            var taxConfig = TaxConfigFactory.GetConfig(DateTime.Now.Year);
            var tax = taxConfig.CalculateTax(taxableIncome);
            return tax;
        }
    }

    public decimal YearlyTotalAfterDeductions => YearlyTotal - YearlyKv - YearlyEks;

    public MainWindowViewModel() {
        LoadConfig();
        LoadInvoices();
    }

    partial void OnCurrentKvChanged(decimal value) {
        SaveConfig();
    }

    [RelayCommand]
    private void AddInvoice() {
        using var db = new SteuerkombinatDbContext();
        var invoice = new Invoice {
            Amount = InvoiceAmount,
            InvoiceNumber = InvoiceNumber,
            Date = DateTime.Now
        };
        db.Invoices.Add(invoice);
        db.SaveChanges();

        LoadInvoices();

        // Clear input fields
        InvoiceAmount = 0;
        InvoiceNumber = string.Empty;
    }

    [RelayCommand]
    private void DeleteInvoice(Invoice invoice) {
        using var db = new SteuerkombinatDbContext();
        db.Invoices.Remove(invoice);
        db.SaveChanges();

        LoadInvoices();
    }

    private void LoadConfig() {
        using var db = new SteuerkombinatDbContext();
        var config = db.TaxConfigs.FirstOrDefault();
        if (config == null) {
            // Create default config
            config = new TaxConfig { CurrentKv = 0 };
            db.TaxConfigs.Add(config);
            db.SaveChanges();
        }
        CurrentKv = config.CurrentKv;
    }

    private void SaveConfig() {
        using var db = new SteuerkombinatDbContext();
        var config = db.TaxConfigs.FirstOrDefault();
        if (config != null) {
            config.CurrentKv = CurrentKv;
            db.SaveChanges();
        }
    }

    private void LoadInvoices() {
        using var db = new SteuerkombinatDbContext();
        var invoices = db.Invoices.OrderByDescending(i => i.Date).ToList();
        Invoices.Clear();
        foreach (var invoice in invoices) {
            Invoices.Add(invoice);
        }

        OnPropertyChanged(nameof(YearlyTotal));
        OnPropertyChanged(nameof(YearlyKv));
        OnPropertyChanged(nameof(YearlyEks));
        OnPropertyChanged(nameof(YearlyTotalAfterDeductions));
    }
}