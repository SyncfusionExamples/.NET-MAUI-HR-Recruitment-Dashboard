using System;
using System.Collections.Generic;
using System.Text;

namespace HrDashboardBlogSample.Model
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
}
