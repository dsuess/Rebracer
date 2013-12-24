﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace WpfGrowlNotification {
	public partial class GrowlNotifications {
		private const byte MAX_NOTIFICATIONS = 4;
		private readonly ObservableCollection<Notification> buffer = new ObservableCollection<Notification>();

		private readonly ObservableCollection<Notification> currentNotifications = new ObservableCollection<Notification>();

		public GrowlNotifications() {
			InitializeComponent();
			NotificationsControl.DataContext = currentNotifications;
		}

		public void AddNotification(Notification notification) {
			if (currentNotifications.Count + 1 > MAX_NOTIFICATIONS)
				buffer.Add(notification);
			else
				currentNotifications.Add(notification);

			//Show window if there're notifications
			if (currentNotifications.Count > 0 && !IsActive)
				Show();
		}

		public void RemoveNotification(Notification notification) {
			if (currentNotifications.Contains(notification))
				currentNotifications.Remove(notification);

			if (buffer.Count > 0) {
				currentNotifications.Add(buffer[0]);
				buffer.RemoveAt(0);
			}

			//Close window if there's nothing to show
			if (currentNotifications.Count < 1)
				Hide();
		}
		private void NotificationWindow_Loaded(object sender, RoutedEventArgs e) {
			var element = (Border)sender;

			// When the hide animation starts, wait until
			// it finishes, then remove the notification.
			// I wish I could listen the Completed event,
			// but I can only get it from a resource, and
			// I can't figure out how to find the target.
			SizeChangedEventHandler sizeChangedHandler = null;
			sizeChangedHandler = async (s, se) => {
				if (se.PreviousSize.Height == 0)
					return;
				element.SizeChanged -= sizeChangedHandler;

				while (element.Height > 0)
					await Task.Delay(500);
				RemoveNotification((Notification)element.DataContext);
			};
			element.SizeChanged += sizeChangedHandler;
		}
	}

	class TitleButton : GlyphButton { }
	// Stolen from Microsoft.VisualStudio.PlatformUI.EnvironmentColors
	// in Microsoft.VisualStudio.Shell.11.0 & higher.  This way, we do
	// not reference any VS11+ DLLs, avoiding all conflicts.
	// The ThemeResourceKey class is OK to reference; it is defined in
	// Microsoft.VisualStudio.Shell.Immutable.11.0, which doesn't have
	// conflicting references.  We will probably need to ship that DLL
	// for pure VS2010 support.

	// TODO: Create ResourceDictionary to supply these keys in VS2010.
	public static class EnvironmentColors {
		private static readonly Guid Category = new Guid("{624ed9c3-bdfd-41fa-96c3-7c824ea32e3d}");
		private static ThemeResourceKey _ComboBoxMouseOverBackgroundBeginBrushKey;
		private static ThemeResourceKey _ToolWindowButtonDownActiveGlyphBrushKey;
		private static ThemeResourceKey _ToolWindowButtonDownBorderBrushKey;
		private static ThemeResourceKey _ToolWindowButtonDownBorderColorKey;
		private static ThemeResourceKey _ToolWindowButtonDownBrushKey;
		private static ThemeResourceKey _ToolWindowButtonHoverActiveGlyphBrushKey;
		private static ThemeResourceKey _ToolWindowButtonHoverActiveBrushKey;
		private static ThemeResourceKey _ToolWindowButtonHoverActiveBorderBrushKey;
		private static ThemeResourceKey _ToolWindowButtonInactiveGlyphBrushKey;
		private static ThemeResourceKey _MainWindowButtonActiveBorderBrushKey;
		private static ThemeResourceKey _ComboBoxMouseOverBorderBrushKey;
		private static ThemeResourceKey _ComboBoxMouseOverTextBrushKey;

		// Regex replacements for ILSpy dump:
		// ThemeResourceKey ([a-zA-Z0-9_]+);\r\n\s*if \(\(\1 = EnvironmentColors.([_a-zA-Z]+)\) == null\) {\r\n\s*\1 = \(EnvironmentColors.\2 = new ThemeResourceKey\(EnvironmentColors.Category, (.+)\)\);\r\n\s*\}\r\n\s*return \1;
		// return EnsureKey(ref $2, $3);
		// return $2\n\t\t\t\t\t?? ($2 = $3);

		static ThemeResourceKey EnsureKey(ref ThemeResourceKey field, string name, ThemeResourceKeyType type) {
			return field
				?? (field = new ThemeResourceKey(EnvironmentColors.Category, name, type));
		}

		public static ThemeResourceKey ComboBoxMouseOverBackgroundBeginBrushKey {
			get {
				return EnsureKey(ref _ComboBoxMouseOverBackgroundBeginBrushKey, "ComboBoxMouseOverBackgroundBegin", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ComboBoxMouseOverTextBrushKey {
			get {
				return EnsureKey(ref _ComboBoxMouseOverTextBrushKey, "ComboBoxMouseOverText", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ComboBoxMouseOverBorderBrushKey {
			get {
				return EnsureKey(ref _ComboBoxMouseOverBorderBrushKey, "ComboBoxMouseOverBorder", ThemeResourceKeyType.BackgroundBrush);
			}
		}

		public static ThemeResourceKey MainWindowButtonActiveBorderBrushKey {
			get {
				return EnsureKey(ref _MainWindowButtonActiveBorderBrushKey, "MainWindowButtonActiveBorder", ThemeResourceKeyType.BackgroundBrush);
			}
		}



		public static ThemeResourceKey ToolWindowButtonInactiveGlyphBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonInactiveGlyphBrushKey, "ToolWindowButtonInactiveGlyph", ThemeResourceKeyType.BackgroundBrush);
			}
		}


		public static ThemeResourceKey ToolWindowButtonHoverActiveBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonHoverActiveBrushKey, "ToolWindowButtonHoverActive", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ToolWindowButtonHoverActiveBorderBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonHoverActiveBorderBrushKey, "ToolWindowButtonHoverActiveBorder", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ToolWindowButtonHoverActiveGlyphBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonHoverActiveGlyphBrushKey, "ToolWindowButtonHoverActiveGlyph", ThemeResourceKeyType.BackgroundBrush);
			}
		}


		public static ThemeResourceKey ToolWindowButtonDownBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonDownBrushKey, "ToolWindowButtonDown", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ToolWindowButtonDownBorderColorKey {
			get {
				return EnsureKey(ref _ToolWindowButtonDownBorderColorKey, "ToolWindowButtonDownBorder", ThemeResourceKeyType.BackgroundColor);
			}
		}
		public static ThemeResourceKey ToolWindowButtonDownBorderBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonDownBorderBrushKey, "ToolWindowButtonDownBorder", ThemeResourceKeyType.BackgroundBrush);
			}
		}
		public static ThemeResourceKey ToolWindowButtonDownActiveGlyphBrushKey {
			get {
				return EnsureKey(ref _ToolWindowButtonDownActiveGlyphBrushKey, "ToolWindowButtonDownActiveGlyph", ThemeResourceKeyType.BackgroundBrush);
			}
		}
	}
}
