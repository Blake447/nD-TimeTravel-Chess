namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// TreeGraph component for the DataTypeGameMode.
	/// </summary>
	public partial class TreeGraphComponentDataTypeGameMode : UIWidgets.TreeGraphComponent<DataTypeGameMode>
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
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				graphicsForeground = new UnityEngine.UI.Graphic[] { UIWidgets.UtilitiesUI.GetGraphic(GameType), UIWidgets.UtilitiesUI.GetGraphic(type), UIWidgets.UtilitiesUI.GetGraphic(multiverse), UIWidgets.UtilitiesUI.GetGraphic(dimensions), UIWidgets.UtilitiesUI.GetGraphic(players), UIWidgets.UtilitiesUI.GetGraphic(scene_index),  };
				if (!UIWidgets.UtilitiesCollections.AllNull(graphicsForeground))
				{
					GraphicsForegroundVersion = 1;
				}
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(UIWidgets.TreeNode<DataTypeGameMode> node)
		{
			Node = node;

			if (GameType != null)
			{
				GameType.text = Node.Item.GameType;
			}

			if (type != null)
			{
				type.text = Node.Item.type;
			}

			if (multiverse != null)
			{
				multiverse.text = Node.Item.multiverse;
			}

			if (dimensions != null)
			{
				dimensions.text = Node.Item.dimensions;
			}

			if (players != null)
			{
				players.text = Node.Item.players.ToString();
			}

			if (scene_index != null)
			{
				scene_index.text = Node.Item.scene_index.ToString();
			}
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
		}
	}
}