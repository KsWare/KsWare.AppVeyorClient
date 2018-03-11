

// REFERENCE: System.Activities
// REFERENCE: System.Activities.Core.Presentation
// REFERENCE: System.Activities.Presentation

using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KsWare.AppVeyorClient.Shared.WpfPropertyGrid {

	/// <summary>WPF Native PropertyGrid class, uses Workflow Foundation's PropertyInspector</summary>
	public class PropertyGrid : Grid {

		#region Private fields

		private readonly WorkflowDesigner _designer;
		private readonly MethodInfo _refreshMethod;
		private readonly MethodInfo _onSelectionChangedMethod;
		private readonly MethodInfo _isInAlphaViewMethod;
		private readonly TextBlock _selectionTypeLabel;
		private readonly Control _propertyToolBar;
		private readonly Border _helpText;
		private readonly GridSplitter _splitter;
		private double _helpTextHeight = 60;

		#endregion

		#region Public properties

		/// <summary>Get or sets the selected object. Can be null.</summary>
		public object SelectedObject {
			get => GetValue(SelectedObjectProperty);
			set => SetValue(SelectedObjectProperty, value);
		}

		/// <summary>Get or sets the selected object collection. Returns empty array by default.</summary>
		public object[] SelectedObjects {
			get => GetValue(SelectedObjectsProperty) as object[];
			set => SetValue(SelectedObjectsProperty, value);
		}

		/// <summary>XAML information with PropertyGrid's font and color information</summary>
		/// <seealso>Documentation for WorkflowDesigner.PropertyInspectorFontAndColorData</seealso>
		public string FontAndColorData { set => _designer.PropertyInspectorFontAndColorData = value; }

		/// <summary>Shows the description area on the top of the control</summary>
		public bool HelpVisible { get => (bool) GetValue(HelpVisibleProperty); set => SetValue(HelpVisibleProperty, value); }

		/// <summary>Shows the tolbar on the top of the control</summary>
		public bool ToolbarVisible {
			get => (bool) GetValue(ToolbarVisibleProperty);
			set => SetValue(ToolbarVisibleProperty, value);
		}

		public PropertySort PropertySort {
			get => (PropertySort) GetValue(PropertySortProperty);
			set => SetValue(PropertySortProperty, value);
		}

		#endregion

		#region Dependency properties registration

		public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject",
			typeof(object), typeof(PropertyGrid),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				SelectedObjectPropertyChanged));

		public static readonly DependencyProperty SelectedObjectsProperty = DependencyProperty.Register("SelectedObjects",
			typeof(object[]), typeof(PropertyGrid),
			new FrameworkPropertyMetadata(new object[0], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				SelectedObjectsPropertyChanged, CoerceSelectedObjects));

		public static readonly DependencyProperty HelpVisibleProperty = DependencyProperty.Register("HelpVisible",
			typeof(bool), typeof(PropertyGrid),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				HelpVisiblePropertyChanged));

		public static readonly DependencyProperty ToolbarVisibleProperty = DependencyProperty.Register("ToolbarVisible",
			typeof(bool), typeof(PropertyGrid),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				ToolbarVisiblePropertyChanged));

		public static readonly DependencyProperty PropertySortProperty = DependencyProperty.Register("PropertySort",
			typeof(PropertySort), typeof(PropertyGrid),
			new FrameworkPropertyMetadata(PropertySort.CategorizedAlphabetical,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertySortPropertyChanged));

		#endregion

		#region Dependency properties events

		private static object CoerceSelectedObject(DependencyObject d, object value) {
			var pg = (PropertyGrid) d;

			var collection = pg.GetValue(SelectedObjectsProperty) as object[];

			return collection.Length == 0 ? null : value;
		}

		private static object CoerceSelectedObjects(DependencyObject d, object value) {
			var pg = (PropertyGrid) d;

			var single = pg.GetValue(SelectedObjectsProperty);

			return single == null ? new object[0] : value;
		}

		private static void SelectedObjectPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			var pg = (PropertyGrid) source;
			pg.CoerceValue(SelectedObjectsProperty);

			if (e.NewValue == null) {
				pg._onSelectionChangedMethod.Invoke(pg._designer.PropertyInspectorView, new object[] {null});
				pg._selectionTypeLabel.Text = string.Empty;
			}
			else {
				var context = new EditingContext();
				var mtm     = new ModelTreeManager(context);
				mtm.Load(e.NewValue);
				var selection = Selection.Select(context, mtm.Root);

				pg._onSelectionChangedMethod.Invoke(pg._designer.PropertyInspectorView, new object[] {selection});
				pg._selectionTypeLabel.Text = e.NewValue.GetType().Name;
			}

			pg.ChangeHelpText(string.Empty, string.Empty);
		}

		private static void SelectedObjectsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			var pg = (PropertyGrid) source;
			pg.CoerceValue(SelectedObjectsProperty);

			var collection = (object[]) e.NewValue;

			if (collection.Length == 0) {
				pg._onSelectionChangedMethod.Invoke(pg._designer.PropertyInspectorView, new object[] {null});
				pg._selectionTypeLabel.Text = string.Empty;
			}
			else {
				var  same  = true;
				Type first = null;

				var       context   = new EditingContext();
				var       mtm       = new ModelTreeManager(context);
				Selection selection = null;

				// Accumulates the selection and determines the type to be shown in the top of the PG
				for (var i = 0; i < collection.Length; i++) {
					mtm.Load(collection[i]);
					if (i == 0) {
						selection = Selection.Select(context, mtm.Root);
						first     = collection[0].GetType();
					}
					else {
						selection = Selection.Union(context, mtm.Root);
						if (!collection[i].GetType().Equals(first)) same = false;
					}
				}

				pg._onSelectionChangedMethod.Invoke(pg._designer.PropertyInspectorView, new object[] {selection});
				pg._selectionTypeLabel.Text = same ? first.Name + " <multiple>" : "Object <multiple>";
			}

			pg.ChangeHelpText(string.Empty, string.Empty);
		}

		private static void HelpVisiblePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			var pg = (PropertyGrid) source;

			if (e.NewValue != e.OldValue) {
				if (e.NewValue.Equals(true)) {
					pg.RowDefinitions[1].Height = new GridLength(5);
					pg.RowDefinitions[2].Height = new GridLength(pg._helpTextHeight);
				}
				else {
					pg._helpTextHeight          = pg.RowDefinitions[2].Height.Value;
					pg.RowDefinitions[1].Height = new GridLength(0);
					pg.RowDefinitions[2].Height = new GridLength(0);
				}
			}
		}

		private static void ToolbarVisiblePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			var pg = (PropertyGrid) source;
			pg._propertyToolBar.Visibility = e.NewValue.Equals(true) ? Visibility.Visible : Visibility.Collapsed;
		}

		private static void PropertySortPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			var pg   = (PropertyGrid) source;
			var sort = (PropertySort) e.NewValue;

			var isAlpha = (sort == PropertySort.Alphabetical || sort == PropertySort.NoSort);
			pg._isInAlphaViewMethod.Invoke(pg._designer.PropertyInspectorView, new object[] {isAlpha});
		}

		#endregion

		/// <summary>Default constructor, creates the UIElements including a PropertyInspector</summary>
		public PropertyGrid() {
			ColumnDefinitions.Add(new ColumnDefinition());
			RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)});
			RowDefinitions.Add(new RowDefinition() {Height = new GridLength(0)});
			RowDefinitions.Add(new RowDefinition() {Height = new GridLength(0)});

			_designer = new WorkflowDesigner();
			var title = new TextBlock() {
				Visibility   = Visibility.Visible,
				TextWrapping = TextWrapping.NoWrap,
				TextTrimming = TextTrimming.CharacterEllipsis,
				FontWeight   = FontWeights.Bold
			};
			var descrip = new TextBlock() {
				Visibility   = Visibility.Visible,
				TextWrapping = TextWrapping.Wrap,
				TextTrimming = TextTrimming.CharacterEllipsis
			};
			var dock = new DockPanel() {
				Visibility    = Visibility.Visible,
				LastChildFill = true,
				Margin        = new Thickness(3, 0, 3, 0)
			};

			title.SetValue(DockPanel.DockProperty, Dock.Top);
			dock.Children.Add(title);
			dock.Children.Add(descrip);
			_helpText = new Border() {
				Visibility      = Visibility.Visible,
				BorderBrush     = SystemColors.ActiveBorderBrush,
				Background      = SystemColors.ControlBrush,
				BorderThickness = new Thickness(1),
				Child           = dock
			};
			_splitter = new GridSplitter() {
				Visibility          = Visibility.Visible,
				ResizeDirection     = GridResizeDirection.Rows,
				Height              = 5,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			var inspector = _designer.PropertyInspectorView;
			inspector.Visibility = Visibility.Visible;
			inspector.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

			_splitter.SetValue(RowProperty,    1);
			_splitter.SetValue(ColumnProperty, 0);

			_helpText.SetValue(RowProperty,    2);
			_helpText.SetValue(ColumnProperty, 0);

			var binding = new Binding("Parent.Background");
			title.SetBinding(BackgroundProperty, binding);
			descrip.SetBinding(BackgroundProperty, binding);

			Children.Add(inspector);
			Children.Add(_splitter);
			Children.Add(_helpText);

			var inspectorType = inspector.GetType();
			var props = inspectorType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			                                        BindingFlags.DeclaredOnly);

			var methods = inspectorType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			                                       BindingFlags.DeclaredOnly);

			_refreshMethod = inspectorType.GetMethod("RefreshPropertyList",
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			_isInAlphaViewMethod = inspectorType.GetMethod("set_IsInAlphaView",
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			_onSelectionChangedMethod = inspectorType.GetMethod("OnSelectionChanged",
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			_selectionTypeLabel = (TextBlock) inspectorType
					.GetMethod("get_SelectionTypeLabel",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Invoke(inspector, new object[0]);
			_propertyToolBar = (Control) inspectorType
					.GetMethod("get_PropertyToolBar",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Invoke(inspector, new object[0]);
			inspectorType.GetEvent("GotFocus").AddEventHandler(this,
				Delegate.CreateDelegate(typeof(RoutedEventHandler), this, "GotFocusHandler", false));

			_selectionTypeLabel.Text = string.Empty;
		}

		/// <summary>Updates the PropertyGrid's properties</summary>
		public void RefreshPropertyList() { _refreshMethod.Invoke(_designer.PropertyInspectorView, new object[] {false}); }

		/// <summary>Traps the change of focused property and updates the help text</summary>
		/// <param name="sender">Not used</param>
		/// <param name="args">Points to the source control containing the selected property</param>
		private void GotFocusHandler(object sender, RoutedEventArgs args) {
			//if (args.OriginalSource is TextBlock)
			//{
			var title              = string.Empty;
			var descrip            = string.Empty;
			var theSelectedObjects = (object[]) GetValue(SelectedObjectsProperty);

			if (theSelectedObjects != null && theSelectedObjects.Length > 0) {
				var first = theSelectedObjects[0].GetType();
				for (var i = 1; i < theSelectedObjects.Length; i++) {
					if (!theSelectedObjects[i].GetType().Equals(first)) {
						ChangeHelpText(title, descrip);
						return;
					}
				}

				var data      = ((FrameworkElement) args.OriginalSource).DataContext;
				var propEntry = data.GetType().GetProperty("PropertyEntry");
				if (propEntry == null) {
					propEntry = data.GetType().GetProperty("ParentProperty");
				}

				if (propEntry != null) {
					var propEntryValue = propEntry.GetValue(data, null);
					var propName       = (string) propEntryValue.GetType().GetProperty("PropertyName").GetValue(propEntryValue, null);
					title = (string) propEntryValue.GetType().GetProperty("DisplayName").GetValue(propEntryValue, null);
					var property = theSelectedObjects[0].GetType().GetProperty(propName);
					var attrs    = property.GetCustomAttributes(typeof(DescriptionAttribute), true);

					if (attrs != null && attrs.Length > 0) descrip = ((DescriptionAttribute) attrs[0]).Description;
				}
				ChangeHelpText(title, descrip);
			}
			//}
		}

		/// <summary>Changes the text help area contents</summary>
		/// <param name="title">Title in bold</param>
		/// <param name="descrip">Description with ellipsis</param>
		private void ChangeHelpText(string title, string descrip) {
			var dock = (DockPanel) _helpText.Child;
			((TextBlock) dock.Children[0]).Text = title;
			((TextBlock) dock.Children[1]).Text = descrip;
		}
	}

}

// ORIGIN adopted at 2018-03-11
// *********************************************************************
// PLEASE DO NOT REMOVE THIS DISCLAIMER
//
// WpfPropertyGrid - By Jaime Olivares
// July 11, 2011
// Article site: http://www.codeproject.com/KB/grid/WpfPropertyGrid.aspx
// Author site: www.jaimeolivares.com
// License: Code Project Open License (CPOL)
//
// *********************************************************************