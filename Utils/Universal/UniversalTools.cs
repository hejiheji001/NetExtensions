using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using WebGrease.Css.Extensions;
using System.Linq;
using FluentFTP;

namespace Utils.Universal
{
	public static class UniversalTools
	{
		/// <summary>
		/// Return Time In Millisecond
		/// </summary>
		/// <returns></returns>
		public static long GetTime()
		{
			long retval = 0;
			var st = new DateTime(1970, 1, 1);
			var t = (DateTime.Now.ToUniversalTime() - st);
			retval = (long)(t.TotalMilliseconds + 0.5);
			return retval;
		}

		private static FtpClient client;

		private static void FluentFTPInit(string baseurl, string target, string username, string password)
		{
			client = new FtpClient(baseurl)
			{
				Credentials = new NetworkCredential(username, password)
			};

			client.Connect();
		}

		public static void DisconnectFTP()
		{
			if(client != null)
			{
				client.Disconnect();
			}
		}

		public static FtpListItem GetLastestFTPFile(string baseurl, string target, string username, string password)
		{
			return GetLastestFTPFiles(baseurl, target, username, password, 1).FirstOrDefault();
		}

		public static List<FtpListItem> GetLastestFTPFiles(string baseurl, string target, string username, string password, int take = 0)
		{
			var files = GetFTPFiles(baseurl, target, username, password).OrderByDescending(f => client.GetModifiedTime(f.FullName));
			return (take == 0 ? files : files.Take(take)).ToList();
		}

		public static List<FtpListItem> GetFTPFiles(string baseurl, string target, string username, string password)
		{
			if (client == null)
			{
				FluentFTPInit(baseurl, target, username, password);
			}

			return client.GetListing(target).Where(f => f.Type.Equals(FtpFileSystemObjectType.File)).ToList();
		}

		public static void FTPFileDownloader(FtpListItem item, string destination)
		{
			Directory.CreateDirectory(destination);
			client.DownloadFile($@"{destination}\{item.Name}", item.FullName);
		}

		public static string Md5Str(Stream stream)
		{
			var sb = new StringBuilder();
			MD5.Create().ComputeHash(stream).ForEach(t => sb.Append(t.ToString("X2")));
			return sb.ToString();
		}

		public static string Md5Str(string str)
		{
			return Md5Str(str.StringToStream());
		}

		private static Stream StringToStream(this string s)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(s ?? ""));
		}

		public static bool FileDownloader(string url, string suffix, string path)
		{
			string md5;
			return FileDownloaderWithMD5(url, suffix, path, out md5);
		}
		/// <summary>
		/// 从URL下载指定文件 并直接输出该文件的MD5值 （而不是下载完毕后再读取文件做计算）
		/// </summary>
		/// <param name="url">请求的URL</param>
		/// <param name="suffix">文件后缀名</param>
		/// <param name="path">文件保存地址</param>
		/// <param name="md5">文件的MD5值</param>
		/// <returns></returns>
		public static bool FileDownloaderWithMD5(string url, string suffix, string path, out string md5)
		{
			using (var client = new WebClient())
			{
				try
				{
					Directory.CreateDirectory(path);
					var format = suffix ?? url.Substring(url.LastIndexOf('.') + 1);
					var read = new MemoryStream(client.DownloadData(url));
					var name = Md5Str(read);
					var filePath = $"{path}{name}.{format}";
					if (!File.Exists(filePath))
					{
						var write = File.Create(filePath);
						read.WriteTo(write);
					}
					md5 = name;
				}
				catch (Exception)
				{
					md5 = "";
					throw;
				}
				return md5.Equals("");
			}
		}

		public static bool FileDownloaderWithMD5ToFTP(string url, string suffix, string path, out string md5)
		{
			using (var client = new WebClient())
			{
				try
				{
					var format = suffix ?? url.Substring(url.LastIndexOf('.') + 1);
					var read = new MemoryStream(client.DownloadData(url));
					var name = Md5Str(read);
					UploadFile(read, path, $"{name}.{format}");
					md5 = name;
				}
				catch (Exception)
				{
					md5 = "";
					throw;
				}
				return !md5.Equals("");
			}
		}

		public static void SaveToFile(dynamic obj, string path, string fileName)
		{
			if (obj is List<string>)
			{
				using (var outputFile = new StreamWriter(path + fileName))
				{
					foreach (string line in obj)
					{
						outputFile.WriteLine(line);
					}
				}
			}
		}

		public static void SaveToFTPFile(dynamic obj, string path, string fileName)
		{
			if (obj is List<string>)
			{
				var sb = new StringBuilder();
				foreach (string line in obj)
				{
					sb.AppendLine(line);
				}
				var stream = sb.ToString().StringToStream();
				UploadFile(stream, path, fileName);
			}

			if (obj is string)
			{
				var stream = new StringBuilder(obj).ToString().StringToStream();
				UploadFile(stream, path, fileName);
			}
		}

		public static void FTPDownloader(string username, string password, string url, string destination)
		{
			WebClient client = new WebClient()
			{
				Credentials = new NetworkCredential(username, password)
			};
			client.DownloadFile(url, destination);
		}

		public static string LastSubstring(this string str, int index)
		{
			return string.Join("", string.Join("", str.Reverse()).Substring(0, index).Reverse());
		}

		public static string[] Split(this string str, params string[] splitter)
		{
			return str.Split(splitter, StringSplitOptions.None);
		}
	}
}