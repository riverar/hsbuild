/* cogl-1.0.vapi generated by vapigen, do not modify. */

[CCode (cprefix = "Cogl", lower_case_cprefix = "cogl_", gir_namespace = "Cogl", gir_version = "1.0")]
namespace Cogl {
	[Compact]
	[CCode (cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Bitmap : Cogl.Handle {
		public static bool get_size_from_file (string filename, out int width, out int height);
		public static Cogl.Bitmap new_from_file (string filename) throws GLib.Error;
	}
	[Compact]
	[CCode (ref_function = "cogl_handle_ref", unref_function = "cogl_handle_unref", cheader_filename = "cogl/cogl.h")]
	public class Handle {
		[CCode (cname = "cogl_is_material")]
		public bool is_material ();
		[CCode (cname = "cogl_is_offscreen")]
		public bool is_offscreen ();
		[CCode (cname = "cogl_is_program")]
		public bool is_program ();
		[CCode (cname = "cogl_is_shader")]
		public bool is_shader ();
		[CCode (cname = "cogl_is_texture")]
		public bool is_texture ();
		[CCode (cname = "cogl_is_vertex_buffer")]
		public bool is_vertex_buffer ();
	}
	[Compact]
	[CCode (ref_function = "cogl_material_ref", unref_function = "cogl_material_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Material : Cogl.Handle {
		[CCode (has_construct_function = false)]
		public Material ();
		public void get_ambient (out Cogl.Color ambient);
		public void get_color (out Cogl.Color color);
		public void get_diffuse (out Cogl.Color diffuse);
		public void get_emission (out Cogl.Color emission);
		public unowned GLib.List<Cogl.MaterialLayer> get_layers ();
		public int get_n_layers ();
		public float get_shininess ();
		public void get_specular (out Cogl.Color specular);
		public void remove_layer (int layer_index);
		public void set_alpha_test_function (Cogl.MaterialAlphaFunc alpha_func, float alpha_reference);
		public void set_ambient (Cogl.Color ambient);
		public void set_ambient_and_diffuse (Cogl.Color color);
		public bool set_blend (string blend_string) throws Cogl.BlendStringError;
		public void set_blend_constant (Cogl.Color constant_color);
		public void set_color (Cogl.Color color);
		public void set_color4f (float red, float green, float blue, float alpha);
		public void set_color4ub (uchar red, uchar green, uchar blue, uchar alpha);
		public void set_diffuse (Cogl.Color diffuse);
		public void set_emission (Cogl.Color emission);
		public void set_layer (int layer_index, Cogl.Texture texture);
		public bool set_layer_combine (int layer_index, string blend_string) throws Cogl.BlendStringError;
		public void set_layer_combine_constant (int layer_index, Cogl.Color constant);
		public void set_layer_filters (int layer_index, Cogl.MaterialFilter min_filter, Cogl.MaterialFilter mag_filter);
		public void set_layer_matrix (int layer_index, Cogl.Matrix matrix);
		public void set_shininess (float shininess);
		public void set_specular (Cogl.Color specular);
	}
	[Compact]
	[CCode (cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class MaterialLayer : Cogl.Handle {
		public Cogl.MaterialFilter get_mag_filter ();
		public Cogl.MaterialFilter get_min_filter ();
		public unowned Cogl.Texture? get_texture ();
		public Cogl.MaterialLayerType get_type ();
	}
	[Compact]
	[CCode (ref_function = "cogl_offscreen_ref", unref_function = "cogl_offscreen_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Offscreen : Cogl.Handle {
		[CCode (cname = "cogl_pop_draw_buffer")]
		public static void pop_draw_buffer ();
		[CCode (cname = "cogl_push_draw_buffer")]
		public static void push_draw_buffer ();
		[CCode (instance_pos = -1)]
		public void set_draw_buffer (Cogl.BufferTarget target);
		[CCode (has_construct_function = false)]
		public Offscreen.to_texture (Cogl.Texture handle);
	}
	[Compact]
	[CCode (cheader_filename = "cogl/cogl.h")]
	public class PangoFontMap {
		[CCode (type = "PangoFontMap*", has_construct_function = false)]
		public PangoFontMap ();
		public void clear_glyph_cache ();
		public unowned Pango.Context create_context ();
		public unowned Pango.Renderer get_renderer ();
		public bool get_use_mipmapping ();
		public void set_resolution (double dpi);
		public void set_use_mipmapping (bool value);
	}
	[CCode (cheader_filename = "cogl/cogl.h")]
	public class PangoRenderer : Pango.Renderer {
	}
	[Compact]
	[CCode (cheader_filename = "cogl/cogl.h")]
	public class PangoRendererClass {
	}
	[Compact]
	[CCode (ref_function = "cogl_program_ref", unref_function = "cogl_program_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Program : Cogl.Handle {
		[CCode (cname = "cogl_create_program", has_construct_function = false)]
		public Program ();
		public void attach_shader (Cogl.Shader shader_handle);
		public int get_uniform_location (string uniform_name);
		public void link ();
		public static void uniform_1f (int uniform_no, float value);
		public static void uniform_1i (int uniform_no, int value);
		public static void uniform_float (int uniform_no, int size, [CCode (array_length_pos = 2.9)] float[] value);
		public static void uniform_int (int uniform_no, int size, [CCode (array_length_pos = 2.9)] int[] value);
		public static void uniform_matrix (int uniform_no, int size, bool transpose, [CCode (array_length_pos = 2.9)] float[] value);
		public void use ();
	}
	[Compact]
	[CCode (ref_function = "cogl_shader_ref", unref_function = "cogl_shader_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Shader : Cogl.Handle {
		[CCode (cname = "cogl_create_shader", has_construct_function = false)]
		public Shader (Cogl.ShaderType shader_type);
		public void compile ();
		public string get_info_log ();
		public Cogl.ShaderType get_type ();
		public bool is_compiled ();
		public void source (string source);
	}
	[Compact]
	[CCode (ref_function = "cogl_texture_ref", unref_function = "cogl_texture_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class Texture : Cogl.Handle {
		public Texture.from_bitmap (Cogl.Bitmap bmp_handle, Cogl.TextureFlags flags, Cogl.PixelFormat internal_format);
		public Texture.from_data (uint width, uint height, Cogl.TextureFlags flags, Cogl.PixelFormat format, Cogl.PixelFormat internal_format, uint rowstride, [CCode (array_length = false)] uchar[] data);
		public Texture.from_file (string filename, Cogl.TextureFlags flags, Cogl.PixelFormat internal_format) throws GLib.Error;
		public int get_data (Cogl.PixelFormat format, uint rowstride, uchar[] data);
		public Cogl.PixelFormat get_format ();
		public uint get_height ();
		public int get_max_waste ();
		public uint get_rowstride ();
		public uint get_width ();
		public bool is_sliced ();
		public bool set_region (int src_x, int src_y, int dst_x, int dst_y, uint dst_width, uint dst_height, int width, int height, Cogl.PixelFormat format, uint rowstride, uchar[] data);
		public Texture.with_size (uint width, uint height, Cogl.TextureFlags flags, Cogl.PixelFormat internal_format);
	}
	[Compact]
	[CCode (ref_function = "cogl_vertex_buffer_ref", unref_function = "cogl_vertex_buffer_unref", cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class VertexBuffer : Cogl.Handle {
		[CCode (has_construct_function = false)]
		public VertexBuffer (uint n_vertices);
		public void add (string attribute_name, uchar n_components, Cogl.AttributeType type, bool normalized, uint16 stride, void* pointer);
		public void @delete (string attribute_name);
		public void disable (string attribute_name);
		public void draw (Cogl.VerticesMode mode, int first, int count);
		public void draw_elements (Cogl.VerticesMode mode, Cogl.VertexBufferIndices indices, int min_index, int max_index, int indices_offset, int count);
		public void enable (string attribute_name);
		public uint get_n_vertices ();
		public void submit ();
	}
	[Compact]
	[CCode (cname = "CoglHandle", cheader_filename = "cogl/cogl.h")]
	public class VertexBufferIndices : Cogl.Handle {
		public VertexBufferIndices (Cogl.IndicesType indices_type, void* indices_array, int indices_len);
		public static unowned Cogl.VertexBufferIndices get_for_quads (uint n_indices);
		public Cogl.IndicesType get_type ();
	}
	[CCode (type_id = "COGL_TYPE_ANGLE", cheader_filename = "cogl/cogl.h")]
	public struct Angle {
		public Cogl.Fixed cos ();
		public Cogl.Fixed sin ();
		public Cogl.Fixed tan ();
	}
	[CCode (type_id = "COGL_TYPE_COLOR", cheader_filename = "cogl/cogl.h")]
	public struct Color {
		public uchar red;
		public uchar green;
		public uchar blue;
		public uchar alpha;
		public uint32 padding0;
		public uint32 padding1;
		public uint32 padding2;
		[CCode (cname = "cogl_color_new", has_construct_function = false)]
		public Color ();
		public Cogl.Color copy ();
		public static bool equal (void* v1, void* v2);
		public float get_alpha ();
		public uint get_alpha_byte ();
		public float get_alpha_float ();
		public float get_blue ();
		public uint get_blue_byte ();
		public float get_blue_float ();
		public float get_green ();
		public uint get_green_byte ();
		public float get_green_float ();
		public float get_red ();
		public uint get_red_byte ();
		public float get_red_float ();
		public void premultiply ();
		public void set_from_4f (float red, float green, float blue, float alpha);
		public void set_from_4ub (uchar red, uchar green, uchar blue, uchar alpha);
	}
	[CCode (type_id = "COGL_TYPE_FIXED", cheader_filename = "cogl/cogl.h")]
	public struct Fixed {
		public Cogl.Fixed atan2 (Cogl.Fixed b);
		public Cogl.Fixed atani ();
		public Cogl.Fixed cos ();
		public Cogl.Fixed div (Cogl.Fixed b);
		public static Cogl.Fixed log2 (uint x);
		public Cogl.Fixed mul (Cogl.Fixed b);
		public Cogl.Fixed mul_div (Cogl.Fixed b, Cogl.Fixed c);
		public static uint pow (uint x, Cogl.Fixed y);
		public uint pow2 ();
		public Cogl.Fixed sin ();
		public Cogl.Fixed sqrt ();
		public Cogl.Fixed tan ();
	}
	[CCode (type_id = "COGL_TYPE_MATRIX", cheader_filename = "cogl/cogl.h")]
	public struct Matrix {
		public float xx;
		public float yx;
		public float zx;
		public float wx;
		public float xy;
		public float yy;
		public float zy;
		public float wy;
		public float xz;
		public float yz;
		public float zz;
		public float wz;
		public float xw;
		public float yw;
		public float zw;
		public float ww;
		public Matrix.from_array (float[] array);
		public void frustum (float left, float right, float bottom, float top, float z_near, float z_far);
		public unowned float[] get_array ();
		public Matrix.identity ();
		[CCode (cname = "cogl_matrix_multiply")]
		public Matrix.multiply (Cogl.Matrix a, Cogl.Matrix b);
		public void ortho (float left, float right, float bottom, float top, float z_near, float z_far);
		public void perspective (float fov_y, float aspect, float z_near, float z_far);
		public void rotate (float angle, float x, float y, float z);
		public void scale (float sx, float sy, float sz);
		public void transform_point (float x, float y, float z, float w);
		public void translate (float x, float y, float z);
	}
	[CCode (type_id = "COGL_TYPE_TEXTURE_VERTEX", cheader_filename = "cogl/cogl.h")]
	public struct TextureVertex {
		public float x;
		public float y;
		public float z;
		public float tx;
		public float ty;
		public Cogl.Color color;
	}
	[CCode (cprefix = "COGL_ATTRIBUTE_TYPE_", cheader_filename = "cogl/cogl.h")]
	public enum AttributeType {
		BYTE,
		UNSIGNED_BYTE,
		SHORT,
		UNSIGNED_SHORT,
		FLOAT
	}
	[CCode (cprefix = "COGL_BUFFER_BIT_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum BufferBit {
		COLOR,
		DEPTH,
		STENCIL
	}
	[CCode (cprefix = "COGL_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum BufferTarget {
		WINDOW_BUFFER,
		OFFSCREEN_BUFFER
	}
	[CCode (cprefix = "COGL_DEBUG_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum DebugFlags {
		MISC,
		TEXTURE,
		MATERIAL,
		SHADER,
		OFFSCREEN,
		DRAW,
		PANGO,
		RECTANGLES,
		HANDLE,
		BLEND_STRINGS,
		DISABLE_BATCHING,
		FORCE_CLIENT_SIDE_MATRICES,
		DISABLE_VBOS,
		JOURNAL,
		BATCHING,
		DISABLE_SOFTWARE_TRANSFORM
	}
	[CCode (cprefix = "COGL_FEATURE_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum FeatureFlags {
		TEXTURE_RECTANGLE,
		TEXTURE_NPOT,
		TEXTURE_YUV,
		TEXTURE_READ_PIXELS,
		SHADERS_GLSL,
		OFFSCREEN,
		OFFSCREEN_MULTISAMPLE,
		OFFSCREEN_BLIT,
		FOUR_CLIP_PLANES,
		STENCIL_BUFFER,
		VBOS
	}
	[CCode (cprefix = "COGL_FOG_MODE_", cheader_filename = "cogl/cogl.h")]
	public enum FogMode {
		LINEAR,
		EXPONENTIAL,
		EXPONENTIAL_SQUARED
	}
	[CCode (cprefix = "COGL_INDICES_TYPE_UNSIGNED_", cheader_filename = "cogl/cogl.h")]
	public enum IndicesType {
		BYTE,
		SHORT
	}
	[CCode (cprefix = "COGL_MATERIAL_ALPHA_FUNC_", cheader_filename = "cogl/cogl.h")]
	public enum MaterialAlphaFunc {
		NEVER,
		LESS,
		EQUAL,
		LEQUAL,
		GREATER,
		NOTEQUAL,
		GEQUAL,
		ALWAYS
	}
	[CCode (cprefix = "COGL_MATERIAL_FILTER_", cheader_filename = "cogl/cogl.h")]
	public enum MaterialFilter {
		NEAREST,
		LINEAR,
		NEAREST_MIPMAP_NEAREST,
		LINEAR_MIPMAP_NEAREST,
		NEAREST_MIPMAP_LINEAR,
		LINEAR_MIPMAP_LINEAR
	}
	[CCode (cprefix = "COGL_MATERIAL_LAYER_TYPE_", cheader_filename = "cogl/cogl.h")]
	public enum MaterialLayerType {
		TEXTURE
	}
	[CCode (cprefix = "COGL_PIXEL_FORMAT_", cheader_filename = "cogl/cogl.h")]
	public enum PixelFormat {
		ANY,
		A_8,
		RGB_565,
		RGBA_4444,
		RGBA_5551,
		YUV,
		G_8,
		RGB_888,
		BGR_888,
		RGBA_8888,
		BGRA_8888,
		ARGB_8888,
		ABGR_8888,
		RGBA_8888_PRE,
		BGRA_8888_PRE,
		ARGB_8888_PRE,
		ABGR_8888_PRE,
		RGBA_4444_PRE,
		RGBA_5551_PRE
	}
	[CCode (cprefix = "COGL_READ_PIXELS_COLOR_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum ReadPixelsFlags {
		BUFFER
	}
	[CCode (cprefix = "COGL_SHADER_TYPE_", cheader_filename = "cogl/cogl.h")]
	public enum ShaderType {
		VERTEX,
		FRAGMENT
	}
	[CCode (cprefix = "COGL_TEXTURE_", cheader_filename = "cogl/cogl.h")]
	[Flags]
	public enum TextureFlags {
		NONE,
		NO_AUTO_MIPMAP,
		NO_SLICING
	}
	[CCode (cprefix = "COGL_VERTICES_MODE_", cheader_filename = "cogl/cogl.h")]
	public enum VerticesMode {
		POINTS,
		LINE_STRIP,
		LINE_LOOP,
		LINES,
		TRIANGLE_STRIP,
		TRIANGLE_FAN,
		TRIANGLES
	}
	[CCode (cprefix = "COGL_BLEND_STRING_ERROR_", cheader_filename = "cogl/cogl.h")]
	public errordomain BlendStringError {
		PARSE_ERROR,
		ARGUMENT_PARSE_ERROR,
		INVALID_ERROR,
		GPU_UNSUPPORTED_ERROR,
	}
	[CCode (cheader_filename = "cogl/cogl.h", has_target = false)]
	public delegate void FuncPtr ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int AFIRST_BIT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int A_BIT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int BGR_BIT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_0_5;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_1;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_2_PI;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_BITS;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_EPSILON;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_MAX;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_MIN;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_PI;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_PI_2;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_PI_4;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int FIXED_Q;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int PIXEL_FORMAT_24;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int PIXEL_FORMAT_32;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int PREMULT_BIT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int RADIANS_TO_DEGREES;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int SQRTI_ARG_10_PERCENT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int SQRTI_ARG_5_PERCENT;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int SQRTI_ARG_MAX;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int TEXTURE_MAX_WASTE;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int UNORDERED_MASK;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public const int UNPREMULT_MASK;
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void begin_gl ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static bool check_extension (string name, string ext);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clear (Cogl.Color color, ulong buffers);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_ensure ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_pop ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_push (float x_offset, float y_offset, float width, float height);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_push_from_path ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_push_from_path_preserve ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_push_window_rect (float x_offset, float y_offset, float width, float height);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_stack_restore ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void clip_stack_save ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void disable_fog ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static Cogl.Fixed double_to_fixed (double value);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static int double_to_int (double value);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static uint double_to_unit (double value);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void end_gl ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static bool features_available (Cogl.FeatureFlags features);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void flush ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void frustum (float left, float right, float bottom, float top, float z_near, float z_far);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static bool get_backface_culling_enabled ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void get_bitmasks (int red, int green, int blue, int alpha);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static bool get_depth_test_enabled ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static Cogl.FeatureFlags get_features ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void get_modelview_matrix (Cogl.Matrix matrix);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static unowned GLib.OptionGroup get_option_group ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static unowned Cogl.FuncPtr get_proc_address (string name);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void get_projection_matrix (Cogl.Matrix matrix);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void get_viewport (float[] v);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void ortho (float left, float right, float bottom, float top, float near, float far);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void pango_ensure_glyph_cache_for_layout (Pango.Layout layout);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void pango_render_layout (Pango.Layout layout, int x, int y, Cogl.Color color, int flags);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void pango_render_layout_line (Pango.LayoutLine line, int x, int y, Cogl.Color color);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void pango_render_layout_subpixel (Pango.Layout layout, int x, int y, Cogl.Color color, int flags);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_arc (float center_x, float center_y, float radius_x, float radius_y, float angle_1, float angle_2);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_close ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_curve_to (float x_1, float y_1, float x_2, float y_2, float x_3, float y_3);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_ellipse (float center_x, float center_y, float radius_x, float radius_y);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_fill ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_fill_preserve ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_line (float x_1, float y_1, float x_2, float y_2);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_line_to (float x, float y);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_move_to (float x, float y);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_new ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_polygon (float coords, int num_points);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_polyline (float coords, int num_points);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_rectangle (float x_1, float y_1, float x_2, float y_2);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_rel_curve_to (float x_1, float y_1, float x_2, float y_2, float x_3, float y_3);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_rel_line_to (float x, float y);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_rel_move_to (float x, float y);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_round_rectangle (float x_1, float y_1, float x_2, float y_2, float radius, float arc_step);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_stroke ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void path_stroke_preserve ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void perspective (float fovy, float aspect, float z_near, float z_far);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void polygon (Cogl.TextureVertex[] vertices, bool use_color);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void pop_matrix ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void push_matrix ();
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void read_pixels (int x, int y, int width, int height, Cogl.ReadPixelsFlags source, Cogl.PixelFormat format, uchar pixels);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rectangle (float x_1, float y_1, float x_2, float y_2);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rectangle_with_multitexture_coords (float x1, float y1, float x2, float y2, float tex_coords, int tex_coords_len);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rectangle_with_texture_coords (float x1, float y1, float x2, float y2, float tx1, float ty1, float tx2, float ty2);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rectangles ([CCode (array_length = false)] float[] verts, uint n_rects);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rectangles_with_texture_coords ([CCode (array_length = false)] float[] verts, uint n_rects);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void rotate (float angle, float x, float y, float z);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void scale (float x, float y, float z);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_backface_culling_enabled (bool setting);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_depth_test_enabled (bool setting);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_fog (Cogl.Color fog_color, Cogl.FogMode mode, float density, float z_near, float z_far);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_modelview_matrix (Cogl.Matrix matrix);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_projection_matrix (Cogl.Matrix matrix);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_source (Cogl.Material material);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_source_color (Cogl.Color color);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_source_color4f (float red, float green, float blue, float alpha);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_source_color4ub (uchar red, uchar green, uchar blue, uchar alpha);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void set_source_texture (Cogl.Texture texture_handle);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static int sqrti (int x);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void translate (float x, float y, float z);
	[CCode (cheader_filename = "cogl/cogl.h")]
	public static void viewport (uint width, uint height);
}
