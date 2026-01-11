using System;
using Hourglass.Controls;
using Hourglass.Models;

namespace Hourglass.ViewModels;

public class WorkSessionViewModel {
    public WorkSession Session { get; }

    public WorkSessionViewModel(WorkSession session) {
        Session = session;
    }

    public string DisplayText {
        get {
            if (Session.EndDate.HasValue) {
                var duration = Session.EndDate.Value - Session.StartDate;
                string billed = Session.Billed ? "Billed" : "" ;
                return $"{Session.StartDate:g} - {Session.EndDate:g} (Duration: {TimeSpanInput.FormatTimeSpan(duration)}{billed})";
            } 
            else {
                var duration = DateTime.Now - Session.StartDate;
                return $"Started: {Session.StartDate:g} (Duration: {TimeSpanInput.FormatTimeSpan(duration)})";
            }
        }
    }
}
