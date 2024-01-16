namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// ListView drag support for the DataTypeGameMode.
	/// </summary>
	[UnityEngine.RequireComponent(typeof(ListViewComponentDataTypeGameMode))]
	public partial class ListViewDragSupportDataTypeGameMode : UIWidgets.ListViewCustomDragSupport<ListViewDataTypeGameMode, ListViewComponentDataTypeGameMode, DataTypeGameMode>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override DataTypeGameMode GetData(ListViewComponentDataTypeGameMode component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(DataTypeGameMode data)
		{
			DragInfo.SetData(data);
		}
	}
}