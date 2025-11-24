using HrDashboardBlogSample.ViewModels;

namespace HrDashboardBlogSample
{
    public partial class MainPage : ContentPage
    {
        private const double NarrowBreakpoint = 720; // px

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new HrDashboardViewModel();

            SizeChanged += OnSizeChanged;

#if ANDROID || IOS
            // Move the filter to next row to keep header title clear on mobile
            MobileFilterContainer.IsVisible = true;
            DesktopFilterContainer.IsVisible = false;
            Grid.SetRow(MobileFilterContainer, 1);
            Grid.SetColumnSpan(MobileFilterContainer, 2);
#endif
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            if (Width <= 0) return;

            if (Width < NarrowBreakpoint)
                ApplyNarrowLayout();
            else
                ApplyWideLayout();
        }

        // Ensure legend colors match initial series colors by refreshing after load
        private void PipelineChart_Loaded(object sender, EventArgs e)
        {
            // Ensure our custom palette is applied in render order at first load
            if (sender is Syncfusion.Maui.Charts.SfFunnelChart chart)
            {
                ApplyFunnelPalette(chart);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Re-apply palette on appearing to avoid Hot Reload/theme cache issues
            ApplyFunnelPalette(PipelineChart);
        }

        private void ApplyFunnelPalette(Syncfusion.Maui.Charts.SfFunnelChart chart)
        {
            if (chart == null) return;
            chart.PaletteBrushes.Clear();

            // Gradient palette tuned for dark plum card (#3C285C) against background (#6A3D87)
            // Uses cyan → plum → violet hues to match the app theme while keeping stages distinct.

            // High-contrast palette for better readability on dark background
            // Stage 1
            chart.PaletteBrushes.Add(new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#2FD2EC"), 0f),   // 85% contrast aqua
                    new GradientStop(Color.FromArgb("#068AB8"), 1f),   // 85% contrast teal-blue
                }
            });

            // Stage 2
            chart.PaletteBrushes.Add(new LinearGradientBrush
            {
                StartPoint = new Point(0, 1),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#E06CF7"), 0f),   // 85% contrast lavender
                    new GradientStop(Color.FromArgb("#873ABD"), 1f),   // 85% contrast purple
                }
            });

            // Stage 3
            chart.PaletteBrushes.Add(new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#B4DB63"), 0f),   // 85% contrast green
                    new GradientStop(Color.FromArgb("#73BA33"), 1f),   // 85% contrast green
                }
            });

            // Stage 4
            chart.PaletteBrushes.Add(new LinearGradientBrush
            {
                StartPoint = new Point(0, 1),
                EndPoint = new Point(0, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#F59A45"), 0f),   // 85% contrast amber
                    new GradientStop(Color.FromArgb("#D56924"), 1f),   // 85% contrast orange
                }
            });

            // Force a refresh by reassigning ItemsSource
            var vm = BindingContext as HrDashboardBlogSample.ViewModels.HrDashboardViewModel;
            if (vm != null)
            {
                var current = vm.Pipeline.ToList();
                chart.ItemsSource = current;
                // Restore bindings for subsequent changes
                chart.ClearValue(Syncfusion.Maui.Charts.SfFunnelChart.ItemsSourceProperty);
            }
        }

        private ScrollView? GetRootScroll() => this.Content as ScrollView;

        private void ApplyNarrowLayout()
        {
            // 1 column
            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            // Ensure there are enough rows for stacking
            LayoutGrid.RowDefinitions.Clear();
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // header
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // KPI group
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // offer card
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // funnel
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // decline reasons

            // Title/Filter header occupies row 0 already
            Grid.SetColumnSpan(TitleLabel, 1);
            Grid.SetColumn(TitleLabel, 0);
            Grid.SetRow(TitleLabel, 0);

            // KPI group stacked first
            Grid.SetColumn(KpiGroup, 0);
            Grid.SetRow(KpiGroup, 1);

            // Offer card next
            Grid.SetColumn(OfferCard, 0);
            Grid.SetRow(OfferCard, 2);

            // Charts stacked after
            Grid.SetColumn(PipelineCard, 0);
            Grid.SetRow(PipelineCard, 3);

            Grid.SetColumn(DeclineReasonsCard, 0);
            Grid.SetRow(DeclineReasonsCard, 4);

            // Enable horizontal scroll when below breakpoint
            var sv = GetRootScroll();
            if (sv != null)
            {
                sv.Orientation = ScrollOrientation.Both;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
            }
        }

        private void ApplyWideLayout()
        {
            // 2 columns
            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            // Restore original rows
            LayoutGrid.RowDefinitions.Clear();
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // header
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // row 1 KPIs
            LayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // row 2 charts

            // Header label position
            Grid.SetColumnSpan(TitleLabel, 1);
            Grid.SetColumn(TitleLabel, 0);
            Grid.SetRow(TitleLabel, 0);

            // Row 1: KPI group (left) and offer card (right)
            Grid.SetColumn(KpiGroup, 0);
            Grid.SetRow(KpiGroup, 1);

            Grid.SetColumn(OfferCard, 1);
            Grid.SetRow(OfferCard, 1);

            // Row 2: Funnel (left), Decline (right)
            Grid.SetColumn(PipelineCard, 0);
            Grid.SetRow(PipelineCard, 2);

            Grid.SetColumn(DeclineReasonsCard, 1);
            Grid.SetRow(DeclineReasonsCard, 2);

            // Disable horizontal scroll on wide screens
            var sv = GetRootScroll();
            if (sv != null)
            {
                sv.Orientation = ScrollOrientation.Vertical;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
            }

            // Ensure tooltip is hidden on layout changes
            if (RejectedHoverTooltip != null)
                RejectedHoverTooltip.IsVisible = false;
        }
        private void OnRejectedHoverEnter(object? sender, PointerEventArgs e)
        {
#if WINDOWS || MACCATALYST
            if (RejectedHoverTooltip != null)
            {
                // Ensure it's visible and stays anchored at top-right of the KPI area
                RejectedHoverTooltip.IsVisible = true;
            }
#endif
        }

        private void OnRejectedHoverExit(object? sender, PointerEventArgs e)
        {
#if WINDOWS || MACCATALYST
            if (RejectedHoverTooltip != null)
                RejectedHoverTooltip.IsVisible = false;
#endif
        }
    }
}
