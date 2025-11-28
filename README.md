Overview:
This .NET MAUI sample showcases a modern HR Recruitment Dashboard using Syncfusion MAUI Charts. It includes:

•	A department filter (Picker) that updates the entire dashboard
•	KPI cards: Shortlisted, Rejected, Hired, Time to Fill
•	Offer Acceptance Ratio summary
•	Candidate Pipeline (Funnel)
•	Reasons for Candidate Decline (Column chart)
•	Responsive layout with desktop/mobile tweaks

Step 1: Department filter (ComboBox/Picker) 
The department Picker lets users view all KPIs and charts scoped to a specific department. On desktop, it sits next to the title; on mobile, it moves under the title to save space.
Key points:
Binds to Departments list and SelectedDepartment (TwoWay)
Uses a shared style for consistent look
Mobile/desktop placement is controlled by OnPlatform or code-behind

Step 2: Shortlisted Candidates 
Shows how many candidates passed initial screening for the selected department. It’s a simple KPI card with bold value and a title
Key points:
•	Immediate glance metric
•	Styled for readability on dark background
•	Bound to ShortlistedCount

Step 3: Rejected Candidates 
Displays the total rejected count and provides an optional hover/touch tooltip for desktop/tablet explaining derived values.
Key points:
•	Clear count for rejections
•	Optional hover info (desktop) using an overlay
•	Bound to RejectedCount

Step 4: Hired Candidates 
Represents the final stage of the funnel—how many candidates were successfully hired.
Key points:
•	Outcome-focused KPI
•	Helps compare with offers/shortlisted to gauge conversion
•	Bound to HiredCount

Step 5: Time to Fill (Days) 
Shows average days to fill a role—critical for planning and identifying process delays.

Key points:
•	Lower is generally better
•	Track per department to spot bottlenecks
•	Bound to TimeToFillKpi

Step 6: 
Offer Acceptance Ratio Displays acceptance percentage with a quick breakdown of Offers Accepted vs Offers Provided.
Key points:
•	Simple ratio visualized with two supporting numbers
•	Useful to track employer brand and compensation competitiveness
•	Bound to OfferAcceptancePercent, OffersAccepted, OffersProvided

Step 7: 
Candidate Pipeline (Funnel) Visualizes conversion across stages like Applied → Screened → Interviewed → Offered → Hired, highlighting leaks and bottlenecks.

Key points:
•	Funnel chart with labels and legend
•	Optional custom palette applied on Loaded/OnAppearing
•	Bound to Pipeline with Stage and Count


