using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace AerialForWindows.UIHelper {
    public class TabControlEx : TabControl {
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            double maxHeight = 0;
            double maxWidth = 0;
            foreach (TabItem item in Items) {
                var content = item.Content as FrameworkElement;
                if (content == null) continue;

                if (!content.IsMeasureValid)
                    content.Measure(new Size(int.MaxValue, int.MaxValue));

                var height = content.DesiredSize.Height;
                if (maxHeight < height)
                    maxHeight = height;
                var width = content.DesiredSize.Height;
                if (maxWidth < width)
                    maxWidth = width;
            }

            var contentHost = GetTemplateChild("PART_SelectedContentHost") as FrameworkElement;
            Debug.Assert(contentHost != null);
            contentHost.MinHeight = maxHeight;
            contentHost.MinWidth = maxWidth;
        }
    }
}