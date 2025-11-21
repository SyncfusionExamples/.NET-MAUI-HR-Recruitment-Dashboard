using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace HrDashboardBlogSample.ViewModels
{
    public class TimeToFillPoint
    {
        public string Month { get; set; } = string.Empty;
        public double Days { get; set; }
    }

    public class PipelineStage
    {
        public string Stage { get; set; } = string.Empty;
        public double Count { get; set; }
    }

    public class SourceCount
    {
        public string Source { get; set; } = string.Empty;
        public double Hires { get; set; }
    }

    public class DiversityPoint
    {
        public string Department { get; set; } = string.Empty;
        public double Female { get; set; }
        public double Male { get; set; }
        public double NonBinary { get; set; }
    }

    public class Segment
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public class HrDashboardViewModel : INotifyPropertyChanged
    {
        // Data bound to UI
        public ObservableCollection<TimeToFillPoint> TimeToFill { get; } = new();
        public ObservableCollection<PipelineStage> Pipeline { get; } = new();
        public ObservableCollection<SourceCount> HiresBySource { get; } = new();
        public ObservableCollection<DiversityPoint> DiversityFiltered { get; } = new();
        

        // Departments for the Picker (dynamic)
        public ObservableCollection<string> Departments { get; } = new();

        // Decline reasons model/collection
        public class DeclineReason
        {
            public string Reason { get; set; } = string.Empty;
            // Percent acts as weight in master data; in filtered data it represents share of declines
            public double Percent { get; set; }
            public int Count { get; set; }
        }
        public ObservableCollection<DeclineReason> DeclineReasons { get; } = new();

        // Base decline reasons to derive percentages per department
        private readonly Dictionary<string, List<DeclineReason>> _declineReasonsByDept = new();

        // KPI properties
        private int _timeToFillKpi;
        public int TimeToFillKpi
        {
            get => _timeToFillKpi;
            set { if (_timeToFillKpi != value) { _timeToFillKpi = value; OnPropertyChanged(); } }
        }

        private int _offerAcceptancePercent;
        public int OfferAcceptancePercent
        {
            get => _offerAcceptancePercent;
            set { if (_offerAcceptancePercent != value) { _offerAcceptancePercent = value; OnPropertyChanged(); } }
        }

        private int _offersProvided;
        public int OffersProvided
        {
            get => _offersProvided;
            set { if (_offersProvided != value) { _offersProvided = value; OnPropertyChanged(); } }
        }

        private int _offersAccepted;
        public int OffersAccepted
        {
            get => _offersAccepted;
            set { if (_offersAccepted != value) { _offersAccepted = value; OnPropertyChanged(); } }
        }

        // Master data sets by department (including "All")
        private readonly Dictionary<string, List<TimeToFillPoint>> _timeToFillByDept = new();
        private readonly Dictionary<string, List<PipelineStage>> _pipelineByDept = new();
        private readonly Dictionary<string, List<SourceCount>> _hiresBySourceByDept = new();
        

        // Diversity master list
        private readonly ObservableCollection<DiversityPoint> _diversityAll = new();

        // KPI cards (sample values)
        private int _shortlistedCount = 35;
        public int ShortlistedCount
        {
            get => _shortlistedCount;
            set { if (_shortlistedCount != value) { _shortlistedCount = value; OnPropertyChanged(); } }
        }

        private int _rejectedCount = 65;
        public int RejectedCount
        {
            get => _rejectedCount;
            set { if (_rejectedCount != value) { _rejectedCount = value; OnPropertyChanged(); } }
        }

        public int HiredCount => (int)(Pipeline.FirstOrDefault(p => p.Stage == "Hired")?.Count ?? 0);

        // Combo box now uses fixed XAML items; keep SelectedDepartment for binding
        private string _selectedDepartment = "All";
        public string SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (_selectedDepartment != value)
                {
                    _selectedDepartment = value;
                    OnPropertyChanged(nameof(SelectedDepartment));
                    ApplyDepartmentFilter();
                }
            }
        }

        public HrDashboardViewModel()
        {
            // Seed master data per department (including "All")
            SeedMasterData();

            // Initialize with first available selection and notify UI
            if (Departments.Count > 0)
                SelectedDepartment = Departments[0];

            ApplyDepartmentFilter();
        }

        private void SeedMasterData()
        {
            // Diversity master list
            _diversityAll.Add(new DiversityPoint { Department = "Accounts", Female = 6, Male = 9, NonBinary = 1 });
            _diversityAll.Add(new DiversityPoint { Department = "Sales",       Female = 4, Male = 5, NonBinary = 0 });
            _diversityAll.Add(new DiversityPoint { Department = "HR",          Female = 3, Male = 2, NonBinary = 0 });
            _diversityAll.Add(new DiversityPoint { Department = "Marketing",   Female = 5, Male = 4, NonBinary = 1 });

            // Build Departments list from current data
            RefreshDepartments();

            // Time to fill by dept
            _timeToFillByDept["All"] = new List<TimeToFillPoint>
            {
                new() { Month = "Jan", Days = 15 },
                new() { Month = "Feb", Days = 14 },
                new() { Month = "Mar", Days = 13 },
                new() { Month = "Apr", Days = 12 },
                new() { Month = "May", Days = 11 },
                new() { Month = "Jun", Days = 32 },
            };
            _timeToFillByDept["Accounts"] = new List<TimeToFillPoint>
            {
                new() { Month = "Jan", Days = 15 }, new() { Month = "Feb", Days = 15 }, new() { Month = "Mar", Days = 14 },
                new() { Month = "Apr", Days = 13 }, new() { Month = "May", Days = 12 }, new() { Month = "Jun", Days = 9 },
            };
            _timeToFillByDept["Sales"] = new List<TimeToFillPoint>
            {
                new() { Month = "Jan", Days = 13 }, new() { Month = "Feb", Days = 13 }, new() { Month = "Mar", Days = 12 },
                new() { Month = "Apr", Days = 12 }, new() { Month = "May", Days = 11 }, new() { Month = "Jun", Days = 7 },
            };
            _timeToFillByDept["HR"] = new List<TimeToFillPoint>
            {
                new() { Month = "Jan", Days = 14 }, new() { Month = "Feb", Days = 13 }, new() { Month = "Mar", Days = 13 },
                new() { Month = "Apr", Days = 12 }, new() { Month = "May", Days = 11 }, new() { Month = "Jun", Days = 6 },
            };
            _timeToFillByDept["Marketing"] = new List<TimeToFillPoint>
            {
                new() { Month = "Jan", Days = 15 }, new() { Month = "Feb", Days = 14 }, new() { Month = "Mar", Days = 14 },
                new() { Month = "Apr", Days = 13 }, new() { Month = "May", Days = 12 }, new() { Month = "Jun", Days = 10 },
            };

            // Pipeline by dept
            _pipelineByDept["All"] = new List<PipelineStage>
            {
                new() { Stage = "Sourced", Count = 125 },
                new() { Stage = "Interviewed", Count = 85 },
                new() { Stage = "Offered", Count = 45 },
                new() { Stage = "Hired", Count = 22 },
            };
            _pipelineByDept["Accounts"] = new List<PipelineStage>
            {
                new() { Stage = "Sourced", Count = 72 }, new() { Stage = "Interviewed", Count = 48 }, new() { Stage = "Offered", Count = 28 }, new() { Stage = "Hired", Count = 25 },
            };
            _pipelineByDept["Sales"] = new List<PipelineStage>
            {
                new() { Stage = "Sourced", Count = 52 }, new() { Stage = "Interviewed", Count = 34 }, new() { Stage = "Offered", Count = 20 }, new() { Stage = "Hired", Count = 18 },
            };
            _pipelineByDept["HR"] = new List<PipelineStage>
            {
                new() { Stage = "Sourced", Count = 36 }, new() { Stage = "Interviewed", Count = 24 }, new() { Stage = "Offered", Count = 14 }, new() { Stage = "Hired", Count = 12 },
            };
            _pipelineByDept["Marketing"] = new List<PipelineStage>
            {
                new() { Stage = "Sourced", Count = 40 }, new() { Stage = "Interviewed", Count = 26 }, new() { Stage = "Offered", Count = 18 }, new() { Stage = "Hired", Count = 17 },
            };

            // Hires by source by dept
            _hiresBySourceByDept["All"] = new List<SourceCount>
            {
                new() { Source = "LinkedIn", Hires = 8 }, new() { Source = "Referral", Hires = 6 }, new() { Source = "Careers", Hires = 4 }, new() { Source = "Agency", Hires = 2 },
            };
            _hiresBySourceByDept["Accounts"] = new List<SourceCount>
            {
                new() { Source = "LinkedIn", Hires = 10 }, new() { Source = "Referral", Hires = 7 }, new() { Source = "Careers", Hires = 5 }, new() { Source = "Agency", Hires = 3 },
            };
            _hiresBySourceByDept["Sales"] = new List<SourceCount>
            {
                new() { Source = "LinkedIn", Hires = 7 }, new() { Source = "Referral", Hires = 5 }, new() { Source = "Careers", Hires = 4 }, new() { Source = "Agency", Hires = 2 },
            };
            _hiresBySourceByDept["HR"] = new List<SourceCount>
            {
                new() { Source = "LinkedIn", Hires = 4 }, new() { Source = "Referral", Hires = 3 }, new() { Source = "Careers", Hires = 3 }, new() { Source = "Agency", Hires = 2 },
            };
            _hiresBySourceByDept["Marketing"] = new List<SourceCount>
            {
                new() { Source = "LinkedIn", Hires = 6 }, new() { Source = "Referral", Hires = 4 }, new() { Source = "Careers", Hires = 4 }, new() { Source = "Agency", Hires = 3 },
            };

            

            // Decline reasons sample by dept
            _declineReasonsByDept["All"] = new List<DeclineReason>
            {
                new() { Reason = "Salary", Percent = 35.48 },
                new() { Reason = "Other", Percent = 13.98 },
                new() { Reason = "Experience", Percent = 34.41 },
                new() { Reason = "Technical", Percent = 8.6 },
                new() { Reason = "Culture", Percent = 8.6 },
            };
            _declineReasonsByDept["Accounts"] = new List<DeclineReason>
            {
                new() { Reason = "Salary", Percent = 40 }, new() { Reason = "Experience", Percent = 30 }, new() { Reason = "Technical", Percent = 20 }, new() { Reason = "Culture", Percent = 5 }, new() { Reason = "Other", Percent = 5 },
            };
            _declineReasonsByDept["Sales"] = new List<DeclineReason>
            {
                new() { Reason = "Salary", Percent = 32 }, new() { Reason = "Experience", Percent = 26 }, new() { Reason = "Culture", Percent = 18 }, new() { Reason = "Technical", Percent = 10 }, new() { Reason = "Other", Percent = 14 },
            };
            _declineReasonsByDept["HR"] = new List<DeclineReason>
            {
                new() { Reason = "Salary", Percent = 28 }, new() { Reason = "Experience", Percent = 24 }, new() { Reason = "Culture", Percent = 20 }, new() { Reason = "Technical", Percent = 8 }, new() { Reason = "Other", Percent = 20 },
            };
            _declineReasonsByDept["Marketing"] = new List<DeclineReason>
            {
                new() { Reason = "Salary", Percent = 30 }, new() { Reason = "Experience", Percent = 30 }, new() { Reason = "Culture", Percent = 15 }, new() { Reason = "Technical", Percent = 10 }, new() { Reason = "Other", Percent = 15 },
            };
        }

        // Rebuilds the Departments collection from the available data sources and keeps selection valid
        private void RefreshDepartments()
        {
            var set = new HashSet<string>();
            foreach (var d in _diversityAll.Select(x => x.Department)) set.Add(d);
            foreach (var key in _timeToFillByDept.Keys) set.Add(key);
            foreach (var key in _pipelineByDept.Keys) set.Add(key);
            foreach (var key in _hiresBySourceByDept.Keys) set.Add(key);

            set.Remove("All");

            var ordered = new List<string> { "All" };
            ordered.AddRange(set.OrderBy(s => s));

            // Synchronize collection (triggers UI update)
            Departments.Clear();
            foreach (var item in ordered) Departments.Add(item);

            if (!Departments.Contains(SelectedDepartment))
                SelectedDepartment = Departments.FirstOrDefault() ?? "All";
        }

        private void ApplyDepartmentFilter()
        {
            var key = string.IsNullOrWhiteSpace(SelectedDepartment) || SelectedDepartment == "All" ? "All" : SelectedDepartment;

            // Diversity (filter from master)
            DiversityFiltered.Clear();
            if (key == "All")
            {
                foreach (var item in _diversityAll)
                    DiversityFiltered.Add(item);
            }
            else
            {
                foreach (var item in _diversityAll.Where(x => x.Department == key))
                    DiversityFiltered.Add(item);
            }

            // Time to fill
            TimeToFill.Clear();
            if (key == "All")
            {
                // Aggregate by averaging each month across departments
                var depts = Departments.Where(d => d != "All").ToList();
                if (depts.Count > 0)
                {
                    // assume all have same months order
                    var monthOrder = _timeToFillByDept[depts[0]].Select(m => m.Month).ToList();
                    foreach (var month in monthOrder)
                    {
                        var avg = depts
                            .Where(d => _timeToFillByDept.ContainsKey(d))
                            .SelectMany(d => _timeToFillByDept[d].Where(m => m.Month == month))
                            .Select(m => m.Days)
                            .DefaultIfEmpty(0)
                            .Average();
                        TimeToFill.Add(new TimeToFillPoint { Month = month, Days = avg });
                    }
                }
            }
            else if (_timeToFillByDept.TryGetValue(key, out var ttf))
            {
                foreach (var p in ttf) TimeToFill.Add(p);
            }

            // Pipeline
            Pipeline.Clear();
            if (key == "All")
            {
                var totals = new Dictionary<string, double>();
                foreach (var dept in Departments.Where(d => d != "All"))
                {
                    if (_pipelineByDept.TryGetValue(dept, out var list))
                    {
                        foreach (var s in list)
                        {
                            totals[s.Stage] = totals.TryGetValue(s.Stage, out var v) ? v + s.Count : s.Count;
                        }
                    }
                }
                foreach (var kv in new[] { "Sourced", "Interviewed", "Offered", "Hired" })
                {
                    if (totals.ContainsKey(kv))
                        Pipeline.Add(new PipelineStage { Stage = kv, Count = totals[kv] });
                }
            }
            else if (_pipelineByDept.TryGetValue(key, out var pipe))
            {
                foreach (var p in pipe) Pipeline.Add(p);
            }

            // Update KPIs that depend on pipeline
            OnPropertyChanged(nameof(HiredCount));
            ShortlistedCount = (int)(Pipeline.FirstOrDefault(p => p.Stage == "Interviewed")?.Count ?? 0);
            RejectedCount = (int)((Pipeline.FirstOrDefault(p => p.Stage == "Sourced")?.Count ?? 0) - (Pipeline.FirstOrDefault(p => p.Stage == "Hired")?.Count ?? 0));

            // Hires by source
            HiresBySource.Clear();
            if (key == "All")
            {
                var totals = new Dictionary<string, double>();
                foreach (var dept in Departments.Where(d => d != "All"))
                {
                    if (_hiresBySourceByDept.TryGetValue(dept, out var list))
                    {
                        foreach (var s in list)
                        {
                            totals[s.Source] = totals.TryGetValue(s.Source, out var v) ? v + s.Hires : s.Hires;
                        }
                    }
                }
                foreach (var kv in totals)
                    HiresBySource.Add(new SourceCount { Source = kv.Key, Hires = kv.Value });
            }
            else if (_hiresBySourceByDept.TryGetValue(key, out var src))
            {
                foreach (var s in src) HiresBySource.Add(s);
            }
            // HiredCount is derived from Pipeline; notification already triggered above

            // Offer KPIs derived directly from pipeline for consistency
            OffersProvided = (int)(Pipeline.FirstOrDefault(p => p.Stage == "Offered")?.Count ?? 0);
            OffersAccepted = (int)(Pipeline.FirstOrDefault(p => p.Stage == "Hired")?.Count ?? 0);
            var ratio2 = OffersProvided > 0 ? (OffersAccepted * 100.0) / OffersProvided : 0.0;
            OfferAcceptancePercent = (int)System.Math.Round(ratio2, System.MidpointRounding.AwayFromZero);

            // Time to Fill KPI (use latest month value for the dept or aggregated for All)
            if (TimeToFill.Count > 0)
            {
                TimeToFillKpi = (int)System.Math.Round(TimeToFill.Last().Days);
            }

            // Decline reasons (recompute counts proportionally to declined candidates for the dept)
            DeclineReasons.Clear();

            List<DeclineReason>? basis;
            if (key == "All")
            {
                // Build a weighted average of department reason percentages using each dept's declines as weights
                var depts = Departments.Where(d => d != "All").ToList();
                var unionReasons = new HashSet<string>(depts.SelectMany(d => _declineReasonsByDept.ContainsKey(d) ? _declineReasonsByDept[d].Select(r => r.Reason) : Array.Empty<string>()));
                var weightedPercents = new Dictionary<string, double>();
                double totalWeight = 0;
                foreach (var dept in depts)
                {
                    if (_pipelineByDept.TryGetValue(dept, out var pl))
                    {
                        var sourced = pl.FirstOrDefault(p => p.Stage == "Sourced")?.Count ?? 0;
                        var hired = pl.FirstOrDefault(p => p.Stage == "Hired")?.Count ?? 0;
                        var declines = System.Math.Max((int)(sourced - hired), 0);
                        if (declines == 0) continue;
                        totalWeight += declines;
                        if (_declineReasonsByDept.TryGetValue(dept, out var dr))
                        {
                            foreach (var r in unionReasons)
                            {
                                var pct = dr.FirstOrDefault(x => x.Reason == r)?.Percent ?? 0;
                                weightedPercents[r] = weightedPercents.TryGetValue(r, out var v) ? v + pct * declines : pct * declines;
                            }
                        }
                    }
                }
                basis = unionReasons.Select(r => new DeclineReason
                {
                    Reason = r,
                    Percent = totalWeight > 0 ? weightedPercents.GetValueOrDefault(r, 0) / totalWeight : 0
                }).ToList();
            }
            else
            {
                basis = _declineReasonsByDept.ContainsKey(key) ? _declineReasonsByDept[key] : null;
            }

            if (basis != null)
            {
                int totalDeclines = System.Math.Max(RejectedCount, 0);

                var weighted = basis.Where(b => b.Percent > 0).Select(b => new { b.Reason, b.Percent }).ToList();
                int n = weighted.Count;

                if (n == 0 || totalDeclines == 0)
                {
                    foreach (var r in basis)
                        DeclineReasons.Add(new DeclineReason { Reason = r.Reason, Count = 0, Percent = 0 });
                    return;
                }

                double totalWeight = weighted.Sum(w => w.Percent);

                var reasons = weighted.Select(w => w.Reason).ToList();
                var counts = new int[reasons.Count];
                var fractions = new double[reasons.Count];

                if (totalDeclines >= n)
                {
                    for (int i = 0; i < counts.Length; i++) counts[i] = 1;
                    int remaining = totalDeclines - n;

                    for (int i = 0; i < reasons.Count; i++)
                    {
                        double share = totalWeight > 0 ? (weighted[i].Percent / totalWeight) : 0;
                        double exact = remaining * share;
                        int floor = (int)System.Math.Floor(exact);
                        counts[i] += floor;
                        fractions[i] = exact - floor;
                    }

                    int assigned = counts.Sum();
                    int leftover = System.Math.Max(totalDeclines - assigned, 0);
                    var orderIdx = Enumerable.Range(0, reasons.Count)
                                             .OrderByDescending(i => fractions[i])
                                             .Take(leftover);
                    foreach (var idx in orderIdx) counts[idx] += 1;
                }
                else
                {
                    var ordered = weighted.OrderByDescending(w => w.Percent).ToList();
                    for (int i = 0; i < totalDeclines && i < ordered.Count; i++)
                    {
                        int idx = reasons.IndexOf(ordered[i].Reason);
                        if (idx >= 0) counts[idx] = 1;
                    }
                }

                var zeroWeight = basis.Where(b => b.Percent == 0).Select(b => b.Reason).ToList();

                for (int i = 0; i < reasons.Count; i++)
                {
                    var percent = totalDeclines > 0 ? (counts[i] * 100.0) / totalDeclines : 0.0;
                    DeclineReasons.Add(new DeclineReason { Reason = reasons[i], Count = counts[i], Percent = percent });
                }
                foreach (var z in zeroWeight)
                {
                    DeclineReasons.Add(new DeclineReason { Reason = z, Count = 0, Percent = 0 });
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
