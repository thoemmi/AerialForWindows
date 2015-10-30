using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using Cursors = System.Windows.Input.Cursors;

namespace AerialForWindows {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        private HwndSource _winWpfContent;
        private readonly MovieManager _movieManager = new MovieManager();

        private void OnStartup(object sender, StartupEventArgs e) {
            Debug.WriteLine("AerialScreensaver: parameters: " + String.Join(", ", e.Args));

            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s")) {
                ShowScreensaver();
            } else if (e.Args[0].ToLower().StartsWith("/p")) {
                var previewHandle = Convert.ToInt32(e.Args[1]);
                ShowPreview(new IntPtr(previewHandle));
            } else if (e.Args[0].ToLower().StartsWith("/c")) {
                var parentHwnd = IntPtr.Zero;
                if (e.Args[0].Length > 3) {
                    parentHwnd = new IntPtr(int.Parse(e.Args[0].Substring(3)));
                }
                ShowConfiguration(parentHwnd);
            }
        }

        private static void ShowConfiguration(IntPtr parentHwnd) {
            var dialog = new SettingsView();
            WindowInteropHelper windowInteropHelper = null;
            if (parentHwnd != IntPtr.Zero) {
                windowInteropHelper = new WindowInteropHelper(dialog) { Owner = parentHwnd };
            }
            dialog.ShowDialog();
            GC.KeepAlive(windowInteropHelper);
            Current.Shutdown();
        }

        private void ShowPreview(IntPtr previewHwnd) {
            var window = CreateMovieWindow(_movieManager.GetRandomAssetUrl(Settings.UseTimeOfDay));

            var lpRect = new RECT();
            var bGetRect = Win32API.GetClientRect(previewHwnd, ref lpRect);
            Debug.Assert(bGetRect);

            var sourceParams = new HwndSourceParameters("sourceParams") {
                PositionX = 0,
                PositionY = 0,
                Height = lpRect.Bottom - lpRect.Top,
                Width = lpRect.Right - lpRect.Left,
                ParentWindow = previewHwnd,
                WindowStyle = (int) (WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN)
            };

            _winWpfContent = new HwndSource(sourceParams);
            _winWpfContent.Disposed += (_, __) => window.Close();
            _winWpfContent.RootVisual = (Visual) window.Content;
        }

        private void ShowScreensaver() {
            var movieWindowsMode = Settings.MovieWindowsMode;
            string movieUrl = null;
            foreach (var screen in Screen.AllScreens) {
                Window window;
                switch (movieWindowsMode) {
                    case MovieWindowsMode.PrimaryScreenOnly:
                        window = screen.Primary
                            ? CreateMovieWindow(_movieManager.GetRandomAssetUrl(Settings.UseTimeOfDay))
                            : CreateBlackoutWindow();
                        break;
                    case MovieWindowsMode.AllScreensSameMovie:
                        if (movieUrl == null) {
                            movieUrl = _movieManager.GetRandomAssetUrl(Settings.UseTimeOfDay);
                        }
                        window = CreateMovieWindow(movieUrl);
                        break;
                    case MovieWindowsMode.AllScreenDifferentMovies:
                        window = CreateMovieWindow(_movieManager.GetRandomAssetUrl(Settings.UseTimeOfDay));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                window.Cursor = Cursors.None;
                window.Left = screen.Bounds.Left;
                window.Top = screen.Bounds.Top;
                window.Width = screen.Bounds.Width;
                window.Height = screen.Bounds.Height;
                window.Topmost = true;
                window.Loaded += (_, __) => { window.WindowState = WindowState.Maximized; };
                window.MouseDown += (_, __) => { Current.Shutdown(); };
                window.KeyDown += (_, __) => { Current.Shutdown(); };

                window.Show();
            }
        }

        private Window CreateMovieWindow(string movieUrl) {
            var window = CreateBlackoutWindow();
            var mediaElement = new MediaElement {
                Stretch = Stretch.Uniform,
                LoadedBehavior = MediaState.Play,
                Source = new Uri(movieUrl)
            };
            window.Content = mediaElement;
            return window;
        }

        private static Window CreateBlackoutWindow() {
            var window = new Window {
                Background = Brushes.Black,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Title = "Aerial For Windows"
            };
            return window;
        }
    }
}