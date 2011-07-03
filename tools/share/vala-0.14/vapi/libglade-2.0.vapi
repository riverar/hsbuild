/* libglade-2.0.vapi generated by vapigen, do not modify. */

[CCode (cprefix = "Glade", lower_case_cprefix = "glade_")]
namespace Glade {
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class AccelInfo {
		public uint key;
		public Gdk.ModifierType modifiers;
		public weak string @signal;
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class AtkActionInfo {
		public weak string action_name;
		public weak string description;
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class AtkRelationInfo {
		public weak string target;
		public weak string type;
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class ChildInfo {
		public weak Glade.WidgetInfo child;
		public weak string internal_child;
		public uint n_properties;
		public weak Glade.Property properties;
	}
	[Compact]
	[CCode (free_function = "glade_interface_destroy", cheader_filename = "glade/glade.h")]
	public class Interface {
		public uint n_requires;
		public uint n_toplevels;
		public weak GLib.HashTable names;
		public weak string @requires;
		public weak GLib.HashTable strings;
		public weak Glade.WidgetInfo toplevels;
		public void dump (string filename);
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class Property {
		public weak string name;
		public weak string value;
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class SignalInfo {
		public uint after;
		public weak string handler;
		public weak string name;
		public weak string object;
	}
	[Compact]
	[CCode (cheader_filename = "glade/glade.h")]
	public class WidgetInfo {
		public weak Glade.AccelInfo accels;
		public weak Glade.AtkActionInfo atk_actions;
		public weak Glade.Property atk_props;
		public weak Glade.ChildInfo children;
		public weak string classname;
		public uint n_accels;
		public uint n_atk_actions;
		public uint n_atk_props;
		public uint n_children;
		public uint n_properties;
		public uint n_relations;
		public uint n_signals;
		public weak string name;
		public weak Glade.WidgetInfo parent;
		public weak Glade.Property properties;
		public weak Glade.AtkRelationInfo relations;
		public weak Glade.SignalInfo signals;
	}
	[CCode (cheader_filename = "glade/glade.h")]
	public class XML : GLib.Object {
		public weak string filename;
		[CCode (has_construct_function = false)]
		public XML (string fname, string? root, string? domain);
		public unowned Gtk.Widget build_widget (Glade.WidgetInfo info);
		public bool @construct (string fname, string? root, string? domain);
		public bool construct_from_buffer (string buffer, int size, string root, string domain);
		public unowned Gtk.AccelGroup ensure_accel ();
		[CCode (has_construct_function = false)]
		public XML.from_buffer (string buffer, int size, string? root, string? domain);
		public unowned Gtk.Widget get_widget (string name);
		public unowned GLib.List get_widget_prefix (string name);
		public void handle_internal_child (Gtk.Widget parent, Glade.ChildInfo child_info);
		public void handle_widget_prop (Gtk.Widget widget, string prop_name, string value_name);
		[NoWrapper]
		public virtual GLib.Type lookup_type (string gtypename);
		public unowned string relative_file (string filename);
		public void set_common_params (Gtk.Widget widget, Glade.WidgetInfo info);
		public void set_packing_property (Gtk.Widget parent, Gtk.Widget child, string name, string value);
		public void set_toplevel (Gtk.Window window);
		public bool set_value_from_string (GLib.ParamSpec pspec, string str, GLib.Value value);
		public void signal_autoconnect ();
		public void signal_autoconnect_full (Glade.XMLConnectFunc func);
		public void signal_connect (string handlername, GLib.Callback func);
		public void signal_connect_data (string handlername, GLib.Callback func);
		public void signal_connect_full (string handler_name, Glade.XMLConnectFunc func);
	}
	[CCode (cheader_filename = "glade/glade.h", has_target = false)]
	public delegate void ApplyCustomPropFunc (Glade.XML xml, Gtk.Widget widget, string propname, string value);
	[CCode (cheader_filename = "glade/glade.h", has_target = false)]
	public delegate void BuildChildrenFunc (Glade.XML xml, Gtk.Widget parent, Glade.WidgetInfo info);
	[CCode (cheader_filename = "glade/glade.h", has_target = false)]
	public delegate unowned Gtk.Widget FindInternalChildFunc (Glade.XML xml, Gtk.Widget parent, string childname);
	[CCode (cheader_filename = "glade/glade.h", has_target = false)]
	public delegate Gtk.Widget NewFunc (Glade.XML xml, GLib.Type widget_type, Glade.WidgetInfo info);
	[CCode (cheader_filename = "glade/glade.h")]
	public delegate void XMLConnectFunc (string handler_name, GLib.Object object, string signal_name, string signal_data, GLib.Object connect_object, bool after);
	[CCode (cheader_filename = "glade/glade.h")]
	public delegate unowned Gtk.Widget XMLCustomWidgetHandler (Glade.XML xml, string func_name, string name, string string1, string string2, int int1, int int2);
	[CCode (cheader_filename = "glade/glade.h")]
	public const int MODULE_API_VERSION;
	[CCode (cheader_filename = "glade/glade.h")]
	public static int enum_from_string (GLib.Type type, string str);
	[CCode (cheader_filename = "glade/glade.h")]
	public static uint flags_from_string (GLib.Type type, string str);
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned string get_widget_name (Gtk.Widget widget);
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned Glade.XML get_widget_tree (Gtk.Widget widget);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void init ();
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned string module_check_version (int version);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void module_register_widgets ();
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned Glade.Interface parser_parse_buffer (string buffer, int len, string domain);
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned Glade.Interface parser_parse_file (string file, string domain);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void provide (string library);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void register_custom_prop (GLib.Type type, string prop_name, Glade.ApplyCustomPropFunc apply_prop);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void register_widget (GLib.Type type, Glade.NewFunc new_func, Glade.BuildChildrenFunc? build_children, Glade.FindInternalChildFunc? find_internal_child);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void require (string library);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void set_custom_handler (Glade.XMLCustomWidgetHandler handler);
	[CCode (cheader_filename = "glade/glade.h")]
	public static void standard_build_children (Glade.XML self, Gtk.Widget parent, Glade.WidgetInfo info);
	[CCode (cheader_filename = "glade/glade.h")]
	public static unowned Gtk.Widget standard_build_widget (Glade.XML xml, GLib.Type widget_type, Glade.WidgetInfo info);
}