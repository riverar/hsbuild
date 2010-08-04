/* gstreamer-tag-0.10.vapi generated by vapigen, do not modify. */

[CCode (cprefix = "Gst", lower_case_cprefix = "gst_")]
namespace Gst {
	[CCode (cheader_filename = "gst/tag/gsttagdemux.h")]
	public class TagDemux : Gst.Element {
		public void* reserved;
		[NoWrapper]
		public virtual bool identify_tag (Gst.Buffer buffer, bool start_tag, out uint tag_size);
		[NoWrapper]
		public virtual unowned Gst.TagList merge_tags (Gst.TagList start_tags, Gst.TagList end_tags);
		[NoWrapper]
		public virtual Gst.TagDemuxResult parse_tag (Gst.Buffer buffer, bool start_tag, out uint tag_size, out Gst.TagList tags);
	}
	[CCode (cprefix = "GST_TAG_DEMUX_RESULT_", cheader_filename = "gst/tag/gsttagdemux.h")]
	public enum TagDemuxResult {
		BROKEN_TAG,
		AGAIN,
		OK
	}
	[CCode (cprefix = "GST_TAG_IMAGE_TYPE_", cheader_filename = "gst/tag/tag.h")]
	public enum TagImageType {
		NONE,
		UNDEFINED,
		FRONT_COVER,
		BACK_COVER,
		LEAFLET_PAGE,
		MEDIUM,
		LEAD_ARTIST,
		ARTIST,
		CONDUCTOR,
		BAND_ORCHESTRA,
		COMPOSER,
		LYRICIST,
		RECORDING_LOCATION,
		DURING_RECORDING,
		DURING_PERFORMANCE,
		VIDEO_CAPTURE,
		FISH,
		ILLUSTRATION,
		BAND_ARTIST_LOGO,
		PUBLISHER_STUDIO_LOGO
	}
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CDDA_CDDB_DISCID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CDDA_CDDB_DISCID_FULL;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CDDA_MUSICBRAINZ_DISCID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CDDA_MUSICBRAINZ_DISCID_FULL;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CMML_CLIP;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CMML_HEAD;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_CMML_STREAM;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_MUSICBRAINZ_ALBUMARTISTID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_MUSICBRAINZ_ALBUMID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_MUSICBRAINZ_ARTISTID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_MUSICBRAINZ_TRACKID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public const string TAG_MUSICBRAINZ_TRMID;
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static string tag_freeform_string_to_utf8 (string data, int size, string env_vars);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_from_id3_tag (string id3_tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_from_id3_user_tag (string type, string id3_user_tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_from_vorbis_tag (string vorbis_tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_get_language_code_iso_639_1 (string lang_code);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_get_language_code_iso_639_2B (string lang_code);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_get_language_code_iso_639_2T (string lang_code);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_get_language_codes ();
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_get_language_name (string language_code);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static uint tag_id3_genre_count ();
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_id3_genre_get (uint id);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static Gst.Buffer tag_image_data_to_image_buffer (uchar image_data, uint image_data_len, Gst.TagImageType image_type);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static bool tag_list_add_id3_image (Gst.TagList tag_list, uchar image_data, uint image_data_len, uint id3_picture_type);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static Gst.TagList tag_list_from_vorbiscomment_buffer (Gst.Buffer buffer, uchar id_data, uint id_data_length, out string vendor_string);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static Gst.TagList tag_list_new_from_id3v1 (uchar data);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static Gst.Buffer tag_list_to_vorbiscomment_buffer (Gst.TagList list, uchar id_data, uint id_data_length, string vendor_string);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static bool tag_parse_extended_comment (string ext_comment, out string key, out string lang, out string value, bool fail_if_no_key);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static void tag_register_musicbrainz_tags ();
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_to_id3_tag (string gst_tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static GLib.List tag_to_vorbis_comments (Gst.TagList list, string tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static unowned string tag_to_vorbis_tag (string gst_tag);
	[CCode (cheader_filename = "gst/tag/tag.h")]
	public static void vorbis_tag_add (Gst.TagList list, string tag, string value);
}
