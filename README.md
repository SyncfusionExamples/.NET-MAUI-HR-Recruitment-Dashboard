# HR Recruitment Dashboard Using .NET MAUI Toolkit Charts

## Overview
This sample demonstrates a modern, responsive HR Recruitment Dashboard built with .NET MAUI and Syncfusion® .NET MAUI Toolkit Charts. It visualizes key hiring metrics and trends to help HR teams make data-driven decisions across departments and locations.

## Syncfusion .NET MAUI Toolkit Charts
A high-performance charting library for .NET MAUI apps with:
- Broad chart coverage: Funnel, column/bar, line, doughnut/pie, stacked variants, and more.
- Interactivity: Tooltips, data labels, selection, animations.
- Styling: Flexible APIs for axes, legends, labels, palettes, and annotations.

Getting started: https://help.syncfusion.com/maui-toolkit/cartesian-charts/getting-started

## HR Recruitment Dashboard

### Layout overview
Two-column, multi-row responsive layout:
- Title bar: “HR Recruitment Dashboard” with subtitle and department filter
- KPI tiles: Shortlisted, Rejected, Hired, Time to Fill (Days)
- Offer Acceptance card: Headline % with Offers Accepted/Provided
- Charts:
  - Candidate Pipeline (Funnel)
  - Reasons for Candidate Decline (Bar/Column)

Responsive behavior:
- Desktop: Department filter placed beside the title; subtitle visible
- Mobile: Department filter moves under the title; compact spacing

### Dashboard components

#### Department Filter
- Purpose: Segment data by department and update all KPIs/charts instantly.
- Highlights: Bound to Departments and SelectedDepartment (TwoWay); responsive placement using platform checks.

#### KPI Tiles
- Purpose: Provide glanceable metrics for throughput and efficiency.
- Tiles:
  - Shortlisted — Candidates passing initial screening
  - Rejected — Total rejections (optional hover/tap details on desktop/tablet)
  - Hired — Final hires
  - Time to Fill (Days) — Average days to fill roles

#### Offer Acceptance Ratio
- Purpose: Track how effectively offers convert to hires.
- Highlights: Prominent acceptance percentage supported by:
  - Offers Accepted
  - Offers Provided

#### Candidate Pipeline (Funnel)
- Purpose: Visualize stage conversion across the hiring process (e.g., Sourced → Interviewed → Offered → Hired).
- Highlights: Gap spacing for readability, data labels for quick values, optional legend/palette styling.

#### Reasons for Candidate Decline (Bar/Column)
- Purpose: Identify the top drivers of offer declines (e.g., Salary, Experience, Technical, Culture, Other).
- Highlights: Data labels enabled; minimized gridlines; optional custom palette for theme alignment.

### MVVM and Data Binding
- Pattern: MVVM with observable collections for charts and KPI items.
- ViewModel drives:
  - Pipeline stages (stage/count)
  - Decline reasons (reason/count/percent)
  - KPI values (Shortlisted, Rejected, Hired, Time to Fill)
  - Offer metrics (Acceptance %, Offers Accepted/Provided)
  - Department switching that recomputes or swaps bound data

## Output

![Modern HR Recruitment Dashboard Demo](https://github.com/user-attachments/assets/ad716d4f-0f33-4609-b731-f4f2b0871de5)

## Troubleshooting
Path Too Long Exception
- If you encounter this when building, close Visual Studio and rename the repository to a shorter path, then rebuild.

For a step-by-step procedure, refer to the [A Modern HR Recruitment Dashboard Blog](https://www.syncfusion.com/blogs/post/hr-recruitment-dashboard-maui-toolkit).