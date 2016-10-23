using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AerialForWindows.UIHelper {
    public class TabControlEx : TabControl {
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            var contentHost = GetTemplateChild("PART_SelectedContentHost") as FrameworkElement;
            Debug.Assert(contentHost != null);

            contentHost.SetBinding(MinWidthProperty, new Binding {
                Path = new PropertyPath("Items"),
                Source = this,
                Converter = new MaxDimensionConverter(false)
            });
            contentHost.SetBinding(MinHeightProperty, new Binding {
                Path = new PropertyPath("Items"),
                Source = this,
                Converter = new MaxDimensionConverter(true)
            });
        }

        private class MaxDimensionConverter : IValueConverter {
            private readonly bool _checkHeight;

            public MaxDimensionConverter(bool checkHeight) {
                _checkHeight = checkHeight;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
                var items = value as ItemCollection;
                if (items == null) {
                    return null;
                }

                double max = 0;
                foreach (TabItem item in items) {
                    var content = item.Content as FrameworkElement;
                    if (content == null) {
                        continue;
                    }

                    if (!content.IsMeasureValid) {
                        content.Measure(new Size(int.MaxValue, int.MaxValue));
                    }

                    var length = _checkHeight ? content.DesiredSize.Height : content.DesiredSize.Width;
                    if (max < length) {
                        max = length;
                    }
                }

                return max;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}