/* libgnome-menu.vapi generated by vapigen, do not modify. */

[CCode (cprefix = "GMenu", lower_case_cprefix = "gmenu_")]
namespace GMenu {
	[Compact]
	[CCode (ref_function = "gmenu_tree_ref", unref_function = "gmenu_tree_unref", cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class Tree {
		public void add_monitor (GMenu.TreeChangedFunc callback);
		public unowned GMenu.TreeDirectory get_directory_from_path (string path);
		public unowned string get_menu_file ();
		public unowned GMenu.TreeDirectory get_root_directory ();
		public void* get_user_data ();
		public static unowned GMenu.Tree lookup (string menu_file, GMenu.TreeFlags flags);
		public void remove_monitor (GMenu.TreeChangedFunc callback);
		public void set_user_data (void* user_data, GLib.DestroyNotify dnotify);
	}
	[Compact]
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeAlias : GMenu.TreeItem {
		public unowned GMenu.TreeDirectory get_directory ();
		public unowned GMenu.TreeItem get_item ();
	}
	[Compact]
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeDirectory : GMenu.TreeItem {
		public unowned string get_comment ();
		public unowned GLib.SList<GMenu.TreeItem> get_contents ();
		public unowned string get_desktop_file_path ();
		public unowned string get_icon ();
		public bool get_is_nodisplay ();
		public unowned string get_menu_id ();
		public unowned string get_name ();
		public unowned GMenu.Tree get_tree ();
		public unowned string make_path (GMenu.TreeEntry entry);
	}
	[Compact]
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeEntry : GMenu.TreeItem {
		public unowned string get_comment ();
		public unowned string get_desktop_file_id ();
		public unowned string get_desktop_file_path ();
		public unowned string get_exec ();
		public unowned string get_icon ();
		public bool get_is_excluded ();
		public bool get_is_nodisplay ();
		public bool get_launch_in_terminal ();
		public unowned string get_name ();
	}
	[Compact]
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeHeader : GMenu.TreeItem {
		public unowned GMenu.TreeDirectory get_directory ();
	}
	[Compact]
	[CCode (ref_function = "gmenu_tree_item_ref", unref_function = "gmenu_tree_item_unref", cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeItem {
		public unowned GMenu.TreeDirectory get_parent ();
		public GMenu.TreeItemType get_type ();
		public void* get_user_data ();
		public void set_user_data (void* user_data, GLib.DestroyNotify dnotify);
	}
	[Compact]
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public class TreeSeparator : GMenu.TreeItem {
	}
	[CCode (cprefix = "GMENU_TREE_FLAGS_", has_type_id = false, cheader_filename = "gnome-menus/gmenu-tree.h")]
	public enum TreeFlags {
		NONE,
		INCLUDE_EXCLUDED,
		SHOW_EMPTY,
		INCLUDE_NODISPLAY,
		MASK
	}
	[CCode (cprefix = "GMENU_TREE_ITEM_", has_type_id = false, cheader_filename = "gnome-menus/gmenu-tree.h")]
	public enum TreeItemType {
		INVALID,
		DIRECTORY,
		ENTRY,
		SEPARATOR,
		HEADER,
		ALIAS
	}
	[CCode (cheader_filename = "gnome-menus/gmenu-tree.h")]
	public delegate void TreeChangedFunc (GMenu.Tree tree);
}
