namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// Test script for the DataTypeGameMode.
	/// </summary>
	public partial class TestDataTypeGameMode : UIWidgets.WidgetGeneration.TestBase<DataTypeGameMode>
	{
		/// <summary>
		/// Left ListView.
		/// </summary>
		public ListViewDataTypeGameMode LeftListView;

		/// <summary>
		/// Right ListView.
		/// </summary>
		public ListViewDataTypeGameMode RightListView;

		/// <summary>
		/// TileView.
		/// </summary>
		public ListViewDataTypeGameMode TileView;

		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeViewDataTypeGameMode TreeView;

		/// <summary>
		/// Table.
		/// </summary>
		public ListViewDataTypeGameMode Table;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		public TreeGraphDataTypeGameMode TreeGraph;

		/// <summary>
		/// Autocomplete.
		/// </summary>
		public AutocompleteDataTypeGameMode Autocomplete;

		/// <summary>
		/// AutoCombobox.
		/// </summary>
		public AutoComboboxDataTypeGameMode AutoCombobox;

		/// <summary>
		/// Combobox.
		/// </summary>
		public ComboboxDataTypeGameMode Combobox;

		/// <summary>
		/// ComboboxMultiselect.
		/// </summary>
		public ComboboxDataTypeGameMode ComboboxMultiselect;

		/// <summary>
		/// ListView picker.
		/// </summary>
		public PickerListViewDataTypeGameMode PickerListView;

		/// <summary>
		/// TreeView picker.
		/// </summary>
		public PickerTreeViewDataTypeGameMode PickerTreeView;

		UIWidgets.ObservableList<DataTypeGameMode> pickerListViewData;

		UIWidgets.ObservableList<UIWidgets.TreeNode<DataTypeGameMode>> pickerTreeViewNodes;

		/// <summary>
		/// Init.
		/// </summary>
		public void Start()
		{
			var list = GenerateList(4);

			LeftListView.DataSource = list;
			TileView.DataSource = list;

			RightListView.DataSource = GenerateList(15);

			Table.DataSource = GenerateList(50);

			TreeView.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 5, });

			TreeGraph.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 2, 3, 2, });

			Autocomplete.DataSource = GenerateList(50).ToList();

			var ac_list = GenerateList(50);
			AutoCombobox.Combobox.ListView.DataSource = ac_list;
			AutoCombobox.Autocomplete.DataSource = ac_list.ListReference();

			Combobox.ListView.DataSource = GenerateList(20);
			ComboboxMultiselect.ListView.DataSource = GenerateList(20);

			pickerListViewData = GenerateList(20);

			pickerTreeViewNodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 3, });
		}

		/// <summary>
		/// Show ListView picker.
		/// </summary>
		public async void ShowPickerListView()
		{
			var picker = PickerListView.Clone();
			picker.ListView.DataSource = pickerListViewData;
			var item = await picker.ShowAsync(null);
			if (item.Success)
			{
				LeftListView.DataSource.Add(item);
			}
		}

		/// <summary>
		/// Show TreeView picker.
		/// </summary>
		public async void ShowPickerTreeView()
		{
			var picker = PickerTreeView.Clone();
			picker.TreeView.Nodes = pickerTreeViewNodes;
			var item = await picker.ShowAsync(null);
			if (item.Success)
			{
				TreeView.Nodes.Add(item);
			}
		}
	}
}