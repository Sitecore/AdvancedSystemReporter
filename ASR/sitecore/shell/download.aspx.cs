using System;
using System.Web;
using Sitecore.Shell.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using System.IO;
using Sitecore.Web;
using Sitecore.Resources.Media;
using Sitecore;
using Sitecore.IO;
using Sitecore.Text;
using Sitecore.Data;
using Sitecore.Globalization;

namespace ASR.Shell
{
	/// <summary>
	/// Customized download page which allows all users to download files.
	/// </summary>
	/// <remarks>Download page needed to be customized to also allow WCMS Administrators to download reports.</remarks>
	public partial class DownloadPage : SecurePage
	{
		private void DownloadFile(string file)
		{
			Assert.ArgumentNotNull(file, "file");
			file = FileUtil.MapPath(file);
			string str = FileUtil.MapPath("/");
			string str2 = FileUtil.MapPath(Sitecore.Configuration.Settings.DataFolder);
			if ((file.StartsWith(str, StringComparison.InvariantCultureIgnoreCase) || file.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase)) && File.Exists(file))
			{
				FileInfo info = new FileInfo(file);
				WriteCacheHeaders(Path.GetFileName(file), info.Length);
				base.Response.WriteFile(file);
			}
		}

		private void DownloadMediaById(string file)
		{
			Assert.ArgumentNotNull(file, "file");
			UrlString str = new UrlString(HttpUtility.UrlDecode(file));
			ItemUri uri = new ItemUri(str["id"], Language.Parse(str["la"]), Sitecore.Data.Version.Parse(str["vs"]), StringUtil.GetString(new string[] { str["db"], Sitecore.Context.ContentDatabase.Name }));
			Item mediaItem = Database.GetItem(uri);
			if (mediaItem != null)
			{
				WriteMediaItem(mediaItem);
			}
		}

		private void DownloadMediaFile(string file)
		{
			Assert.ArgumentNotNull(file, "file");
			int index = file.IndexOf("~/media/", StringComparison.OrdinalIgnoreCase);
			file = StringUtil.Mid(file, index + "~/media/".Length);
			index = file.LastIndexOf('?');
			if (index >= 0)
			{
				file = StringUtil.Left(file, index);
			}
			index = file.LastIndexOf('.');
			if (index >= 0)
			{
				file = StringUtil.Left(file, index);
			}
			file = FileUtil.MakePath("/sitecore/media library", file);
			Item mediaItem = Sitecore.Context.ContentDatabase.GetItem(file);
			if (mediaItem != null)
			{
				WriteMediaItem(mediaItem);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			Assert.ArgumentNotNull(e, "e");
			base.OnLoad(e);
			string str = StringUtil.GetString(new string[] { base.Request.QueryString["file"] });
			if (!string.IsNullOrEmpty(str))
			{
				if (MediaManager.IsMediaUrl(str))
				{
					DownloadMediaFile(str);
				}
				else if (str.IndexOf("id=", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					DownloadMediaById(str);
				}
				else
				{
					DownloadFile(str);
				}
			}
		}

		private void WriteCacheHeaders(string filename, long length)
		{
			Assert.ArgumentNotNull(filename, "filename");
			base.Response.ClearHeaders();
			base.Response.AddHeader("Content-Type", "application/octet-stream");
			base.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", filename));
			base.Response.AddHeader("Content-Length", length.ToString());
			base.Response.AddHeader("Content-Transfer-Encoding", "binary");
			base.Response.CacheControl = "private";
		}

		private void WriteMediaItem(MediaItem mediaItem)
		{
			Assert.ArgumentNotNull(mediaItem, "mediaItem");
			Stream mediaStream = mediaItem.GetMediaStream();
			string extension = mediaItem.Extension;
			if (!extension.StartsWith("."))
			{
				extension = String.Format(".{0}", extension);
			}
			WriteCacheHeaders(mediaItem.Name + extension, mediaStream.Length);
			WebUtil.TransmitStream(mediaStream, base.Response, Sitecore.Configuration.Settings.Media.StreamBufferSize);
		}
	}


}
