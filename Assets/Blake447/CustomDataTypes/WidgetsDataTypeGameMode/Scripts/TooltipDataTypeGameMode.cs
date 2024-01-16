namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// Tooltip DataTypeGameMode.
	/// </summary>
	public class TooltipDataTypeGameMode : UIWidgets.Tooltip<DataTypeGameMode, TooltipDataTypeGameMode>
	{

		/// <summary>
		/// The GameType.
		/// </summary>
		public UIWidgets.TextAdapter GameType;

		/// <summary>
		/// The type.
		/// </summary>
		public UIWidgets.TextAdapter type;

		/// <summary>
		/// The multiverse.
		/// </summary>
		public UIWidgets.TextAdapter multiverse;

		/// <summary>
		/// The dimensions.
		/// </summary>
		public UIWidgets.TextAdapter dimensions;

		/// <summary>
		/// The players.
		/// </summary>
		public UIWidgets.TextAdapter players;

		/// <summary>
		/// The scene_index.
		/// </summary>
		public UIWidgets.TextAdapter scene_index;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public DataTypeGameMode Item
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void SetData(DataTypeGameMode item)
		{
			Item = item;

			UpdateView();
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			if (Item == null)
			{
				return;
			}

			if (GameType != null)
			{
				GameType.text = Item.GameType;
			}

			if (type != null)
			{
				type.text = Item.type;
			}

			if (multiverse != null)
			{
				multiverse.text = Item.multiverse;
			}

			if (dimensions != null)
			{
				dimensions.text = Item.dimensions;
			}

			if (players != null)
			{
				players.text = Item.players.ToString();
			}

			if (scene_index != null)
			{
				scene_index.text = Item.scene_index.ToString();
			}

		}
	}
}