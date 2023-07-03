using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Vue.Data;
using Vue.Models;
using NuGet.LibraryModel;

namespace Syncfusion.EJ2.FileManager.Base.SQLFileProvider
{
	public class SQLFileProvider : SQLFileProviderBase
	{
		string connectionString;
		string tableName = "Library";
		string rootId = "0";
		AccessDetails AccessDetails = new AccessDetails();
		private string accessMessage = string.Empty;
		ItContext _context;
		IConfiguration _configuration;
		// Sets the configuration
		public SQLFileProvider(ItContext itContext, IConfiguration configuration)
		{
			_context = itContext;
			_configuration = configuration;
		}
		// Sets the SQLConnection string, table name and table id
		public void SetSQLConnection(string sqlTableName, string tableID)
		{
			tableName = sqlTableName;
			rootId = tableID;
		}

		public void SetRules(AccessDetails details)
		{
			this.AccessDetails = details;
		}

		public AccessDetails GetRules(string user_id, string? custom)
		{

			AccessDetails accessDetails = new AccessDetails();


			List<AccessRule> accessRules = new List<AccessRule>
			{
				new AccessRule {
					Id = "*",
					Role = user_id,
					Permission = Permission.Deny,
					Read = Permission.Allow,
					WriteContents = Permission.Deny,
					Upload = Permission.Deny,
					Write = Permission.Deny,
					Copy = Permission.Deny,
					Download = Permission.Deny
				},
				new AccessRule {
					Id = "*.*",
					IsFile = true,
					Role = user_id,
					Permission = Permission.Deny,
					Read = Permission.Allow,
					WriteContents = Permission.Deny,
					Upload = Permission.Deny,
					Write = Permission.Deny,
					Copy = Permission.Deny,
					Download = Permission.Deny
				},
			};

			/////PERMISSION
			var permission = _context.LibraryPermissionModel.Where(d => d.user_id == user_id).ToList();
			foreach (var per in permission)
			{
				per.path = GetFilterId(per.item_id.ToString());
			}
			permission = permission.OrderBy(d => d.path).ToList();
			////OWNER
			//var owner = _context.LibraryModel.Where(d => d.user_id == user_id && d.Type == "Folder").ToList();
			//foreach (var per in owner)
			//{
			//	per.FilterPath = GetFilterId(per.ItemId.ToString());
			//}
			//owner = owner.OrderBy(d => d.FilterPath).ToList();

			/////LIST CHECK 
			List<string> list_item = permission.Select(d => d.item_id.ToString()).ToList();
			//var list_owner_id = owner.Select(d => d.ItemId.ToString()).ToList();
			//list_item.AddRange(list_owner_id);

			/////
			foreach (var per in permission)
			{
				var AccessRule = new AccessRule();
				if (per.permission == "Owner")
				{
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/",
						Role = user_id,
						Read = Permission.Allow,
						Permission = Permission.Allow,
						Write = Permission.Allow,
						Copy = Permission.Allow,
						Download = Permission.Allow,
						WriteContents = Permission.Allow,
						Upload = Permission.Allow
					};

					accessRules.Add(AccessRule);
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/*.*",
						IsFile = true,
						Role = user_id,
						Read = Permission.Allow,
						Permission = Permission.Allow,
						Write = Permission.Allow,
						Copy = Permission.Allow,
						Download = Permission.Allow,
						WriteContents = Permission.Allow,
						Upload = Permission.Allow
					};
					accessRules.Add(AccessRule);
				}
				else if (per.permission == "Creator" && custom == "saveEsign")
				{ //== OWNER
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/",
						Role = user_id,
						Read = Permission.Allow,
						Permission = Permission.Allow,
						Write = Permission.Allow,
						Copy = Permission.Allow,
						Download = Permission.Allow,
						WriteContents = Permission.Allow,
						Upload = Permission.Allow
					};
					accessRules.Add(AccessRule);
				}
				else if (per.permission == "Creator")
				{
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/",
						Role = user_id,
						Permission = Permission.Deny,
						Write = Permission.Deny, ////RENAME,DELETE Folder
						Read = Permission.Allow,
						Copy = Permission.Allow,
						Download = Permission.Allow,
						WriteContents = Permission.Allow, /// Create
						Upload = Permission.Allow
					};
					accessRules.Add(AccessRule);
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/*.*",
						IsFile = true,
						Role = user_id,
						Permission = Permission.Deny,
						Write = Permission.Deny, ////RENAME,DELETE Folder
						Read = Permission.Allow,
						Copy = Permission.Allow,
						Download = Permission.Allow,
						WriteContents = Permission.Allow, /// Create
						Upload = Permission.Allow
					};
					accessRules.Add(AccessRule);
				}
				else
				{
					AccessRule = new AccessRule
					{
						Id = per.item_id.ToString() + "/",
						Role = user_id,
						Read = Permission.Allow,
						Write = Permission.Deny,
						Permission = Permission.Deny,
						Copy = Permission.Deny,
						Download = Permission.Deny,
						WriteContents = Permission.Deny,
						Upload = Permission.Deny
					};
					accessRules.Add(AccessRule);
				}
				list_item.Add(per.item_id.ToString() + "/");

			}

			//foreach (var per in owner)
			//{
			//	var AccessRule = new AccessRule();

			//	var filterId = per.FilterPath;
			//	var list_parent = filterId.Split('/').ToList();
			//	var list_add = list_parent.Where(d => d != "").ToList();
			//	foreach (var item in list_add)
			//	{
			//		if (!list_item.Contains(item))
			//		{
			//			list_item.Add(item);
			//			AccessRule = new AccessRule
			//			{
			//				Id = item,
			//				Role = user_id,
			//				Read = Permission.Allow,
			//				Permission = Permission.Deny,
			//				WriteContents = Permission.Deny,
			//				Upload = Permission.Deny,
			//				Write = Permission.Deny,
			//				Copy = Permission.Deny,
			//				Download = Permission.Deny
			//			};
			//			accessRules.Add(AccessRule);
			//		}
			//		else
			//		{
			//			break;
			//		}
			//	}

			//	AccessRule = new AccessRule
			//	{
			//		Id = per.ItemId.ToString() + "/",
			//		Role = user_id,
			//		Read = Permission.Allow,
			//		Permission = Permission.Allow,
			//		Write = Permission.Allow,
			//		Copy = Permission.Allow,
			//		Download = Permission.Allow,
			//		WriteContents = Permission.Allow,
			//		Upload = Permission.Allow
			//	};

			//	list_item.Add(per.ItemId.ToString() + "/");
			//	accessRules.Add(AccessRule);

			//}

			accessDetails.AccessRules = accessRules;
			accessDetails.Role = user_id;
			return accessDetails;
		}
		// Reads the files from SQL table
		public FileManagerResponse GetFiles(string path, bool showHiddenItems, params FileManagerDirectoryContent[] data)
		{
			//int parentID = 0;
			int idRoot = 1;
			int Id = data.Length > 0 ? Int32.Parse(data[0].Id) : idRoot;
			FileManagerResponse readResponse = new FileManagerResponse();
			try
			{


				FileManagerDirectoryContent cwd = new FileManagerDirectoryContent();
				List<FileManagerDirectoryContent> files = new List<FileManagerDirectoryContent>();

				/////
				///CWD
				LibraryModel parent;
				parent = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
				cwd = new FileManagerDirectoryContent
				{
					Name = parent.Name,
					Id = parent.ItemId.ToString(),
					Size = (long)parent.Size,
					IsFile = (bool)parent.IsFile,
					DateModified = (DateTime)parent.DateModified,
					DateCreated = (DateTime)parent.DateCreated,
					Type = parent.Type,
					HasChild = (bool)parent.HasChild,
					ParentID = parent.ParentID.ToString(),
					user_id = parent.user_id,
					FilterPath = data.Length > 0 ? data[0].FilterPath : "",

				};
				AccessPermission permission = GetPermission(cwd.Id, cwd.ParentID, cwd.Name, cwd.IsFile, path);
				cwd.permission = permission;
				cwd.FilterId = GetFilterId(cwd.Id);

				/////
				///File Forlder
				List<LibraryModel> list_2;
				list_2 = _context.LibraryModel.Where(d => d.ParentID == Id && d.DateDeleted == null).ToList();
				foreach (var lib in list_2)
				{
					var childFiles = new FileManagerDirectoryContent
					{
						Name = lib.Name,
						Id = lib.ItemId.ToString(),
						Size = (long)lib.Size,
						IsFile = (bool)lib.IsFile,
						DateModified = (DateTime)lib.DateModified,
						DateCreated = (DateTime)lib.DateCreated,
						Type = GetDefaultExtension(lib.MimeType),
						HasChild = (bool)lib.HasChild,
						ParentID = lib.ParentID.ToString(),
						user_id = lib.user_id,
						url = lib.url,
					};

					AccessPermission permission1 = GetPermission(childFiles.Id, childFiles.ParentID, childFiles.Name, childFiles.IsFile, path);
					childFiles.permission = permission1;
					childFiles.FilterId = GetFilterId(childFiles.Id);
					childFiles.FilterPath = data.Length != 0 ? GetFilterPath(childFiles.Id) : "/Files/";
					files.Add(childFiles);
				}


				readResponse.Files = files;
				readResponse.CWD = cwd;

				if (cwd.permission != null && !cwd.permission.Read)
				{
					readResponse.Files = null;
					accessMessage = cwd.permission.Message;
					throw new UnauthorizedAccessException("'" + cwd.Name + "' is not accessible. You need permission to perform the read action.");
				}
				return readResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				readResponse.Error = error;
				return readResponse;
			}
		}

		protected AccessPermission GetPermission(string id, string parentId, string name, bool isFile, string path)
		{
			AccessPermission FilePermission = new AccessPermission();
			if (isFile)
			{
				if (this.AccessDetails.AccessRules == null)
				{
					return null;
				}
				string nameExtension = Path.GetExtension(name).ToLower();
				AccessRule accessFileRule = new AccessRule();
				foreach (AccessRule fileRule in AccessDetails.AccessRules)
				{
					if (!string.IsNullOrEmpty(fileRule.Id) && fileRule.IsFile && (fileRule.Role == null || fileRule.Role == AccessDetails.Role))
					{
						if (id == fileRule.Id)
						{
							FilePermission = UpdateFileRules(FilePermission, fileRule, name);
						}
						else if (fileRule.Id.IndexOf("*.*") > -1)
						{
							string parentPath = fileRule.Id.Substring(0, fileRule.Id.IndexOf("*.*"));
							if (parentPath == "")
							{
								FilePermission = UpdateFileRules(FilePermission, fileRule, name);
							}
							else
							{
								string idValue = parentPath.Substring(0, parentPath.LastIndexOf("/"));
								bool isAccessId = path.Contains(idValue);
								if (idValue == parentId || isAccessId)
								{
									accessFileRule = UpdateFilePermission(fileRule, parentPath, id);
									FilePermission = UpdateFileRules(FilePermission, accessFileRule, name);
								}
							}
						}
						else if (fileRule.Id.IndexOf("*.") > -1)
						{
							string pathExtension = Path.GetExtension(fileRule.Id).ToLower();
							string parentPath = fileRule.Id.Substring(0, fileRule.Id.IndexOf("*."));
							if (parentPath == "")
							{
								if (pathExtension == nameExtension)
								{
									FilePermission = UpdateFileRules(FilePermission, fileRule, name);
								}
							}
							else
							{
								string idValue = parentPath.Substring(0, parentPath.LastIndexOf("/"));
								bool isAccessId = path.Contains(idValue);
								if ((idValue == parentId || isAccessId) && pathExtension == nameExtension)
								{
									accessFileRule = UpdateFilePermission(fileRule, parentPath, id);
									FilePermission = UpdateFileRules(FilePermission, accessFileRule, name);
								}
							}
						}
					}
				}
				return FilePermission;
			}
			else
			{
				AccessRule accessFolderRule = new AccessRule();

				if (this.AccessDetails.AccessRules == null)
				{
					return null;
				}
				foreach (AccessRule folderRule in AccessDetails.AccessRules)
				{
					if (folderRule.Id != null && folderRule.IsFile == false && (folderRule.Role == null || folderRule.Role == AccessDetails.Role))
					{
						if (id == folderRule.Id)
						{
							FilePermission = UpdateFolderRules(FilePermission, folderRule, name);
						}
						else if (folderRule.Id.IndexOf("*") > -1)
						{
							string parentPath = folderRule.Id.Substring(0, folderRule.Id.IndexOf("*"));
							if (parentPath == "")
							{
								FilePermission = UpdateFolderRules(FilePermission, folderRule, name);
							}
							else
							{
								string idValue = parentPath.Substring(0, parentPath.LastIndexOf("/"));
								if (idValue == parentId)
								{
									accessFolderRule = UpdateFolderPermission(folderRule, parentPath, id);
									FilePermission = UpdateFolderRules(FilePermission, accessFolderRule, name);
								}
							}
						}
						else if (folderRule.Id.IndexOf("/") > -1)
						{
							string idValue = folderRule.Id.Substring(0, folderRule.Id.LastIndexOf("/"));
							bool isAccessId = path.Contains(idValue);
							if (id == idValue || parentId == idValue || isAccessId)
							{
								accessFolderRule = UpdateFolderPermission(folderRule, folderRule.Id, id);
								FilePermission = UpdateFolderRules(FilePermission, accessFolderRule, name);
							}
						}
					}
				}
				return FilePermission;
			}
		}

		protected AccessRule UpdateFilePermission(AccessRule accessRule, string parentPath, string id)
		{
			AccessRule accessFileRule = new AccessRule();
			accessFileRule.Copy = accessRule.Copy;
			accessFileRule.Download = accessRule.Download;
			accessFileRule.Write = accessRule.Write;
			accessFileRule.Id = parentPath + id;
			accessFileRule.Read = accessRule.Read;
			accessFileRule.Role = accessRule.Role;
			accessFileRule.Message = accessRule.Message;
			return accessFileRule;
		}

		protected AccessRule UpdateFolderPermission(AccessRule accessRule, string parentPath, string id)
		{
			AccessRule accessFolderRule = new AccessRule();
			accessFolderRule.Copy = accessRule.Copy;
			accessFolderRule.Download = accessRule.Download;
			accessFolderRule.Write = accessRule.Write;
			accessFolderRule.WriteContents = accessRule.WriteContents;
			accessFolderRule.Id = parentPath + id;
			accessFolderRule.Read = accessRule.Read;
			accessFolderRule.Role = accessRule.Role;
			accessFolderRule.Upload = accessRule.Upload;
			accessFolderRule.Permission = accessRule.Permission;
			accessFolderRule.Message = accessRule.Message;
			return accessFolderRule;
		}

		protected virtual AccessPermission UpdateFileRules(AccessPermission filePermission, AccessRule fileRule, string fileName)
		{
			filePermission.Copy = HasPermission(fileRule.Copy);
			filePermission.Download = HasPermission(fileRule.Download);
			filePermission.Write = HasPermission(fileRule.Write);
			filePermission.Read = HasPermission(fileRule.Read);
			filePermission.Message = string.IsNullOrEmpty(fileRule.Message) ? "'" + fileName + "' is not accessible. You need permission to perform the read action." : fileRule.Message;
			return filePermission;
		}
		protected virtual AccessPermission UpdateFolderRules(AccessPermission folderPermission, AccessRule folderRule, string folderName)
		{
			folderPermission.Copy = HasPermission(folderRule.Copy);
			folderPermission.Download = HasPermission(folderRule.Download);
			folderPermission.Write = HasPermission(folderRule.Write);
			folderPermission.WriteContents = HasPermission(folderRule.WriteContents);
			folderPermission.Read = HasPermission(folderRule.Read);
			folderPermission.Upload = HasPermission(folderRule.Upload);
			folderPermission.Permission = HasPermission(folderRule.Permission);
			folderPermission.Message = string.IsNullOrEmpty(folderRule.Message) ? "'" + folderName + "' is not accessible. You need permission to perform the read action." : folderRule.Message;
			return folderPermission;
		}

		protected bool HasPermission(Permission rule)
		{
			return rule == Permission.Allow ? true : false;
		}

		// Creates a new folder
		public async Task<FileManagerResponse> Create(string path, string name, string user_id, params FileManagerDirectoryContent[] data)
		{
			FileManagerResponse createResponse = new FileManagerResponse();
			try
			{
				FileManagerDirectoryContent createData = new FileManagerDirectoryContent();

				AccessPermission createPermission = GetPermission(data[0].Id, data[0].ParentID, data[0].Name, data[0].IsFile, path);
				if (createPermission != null && (!createPermission.Read || !createPermission.WriteContents))
				{
					accessMessage = createPermission.Message;
					throw new UnauthorizedAccessException("'" + data[0].Name + "' is not accessible. You need permission to perform the writeContents action.");
				}

				try
				{
					var Id = Int32.Parse(data[0].Id);
					int? ParentID = null;
					var folder = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
					if (folder != null)
					{
						ParentID = folder.ParentID;
						folder.HasChild = true;
						_context.Update(folder);
						await _context.SaveChangesAsync();
					}

					var count = _context.LibraryModel.Where(d => d.ParentID == Id && d.Type == "Folder" && d.Name == name.Trim() && d.DateDeleted == null).Count();
					if (count != 0)
					{
						ErrorDetails error = new ErrorDetails();
						error.Code = "400";
						error.Message = "A folder with the name " + name.Trim() + " already exists.";
						createResponse.Error = error;
						return createResponse;
					}
					else
					{

						var newFolder = new LibraryModel
						{
							Name = name.Trim(),
							ParentID = Id,
							Size = 30,
							IsFile = false,
							MimeType = "Folder",
							DateModified = DateTime.Now,
							DateCreated = DateTime.Now,
							HasChild = false,
							IsRoot = false,
							Type = "Folder",
							user_id = user_id,
						};
						_context.Add(newFolder);
						await _context.SaveChangesAsync();

						/// ADD PERMISSION
						var per = new LibraryPermissionModel
						{
							user_id = user_id,
							permission = "Owner",
							item_id = newFolder.ItemId
						};
						_context.Add(per);
						await _context.SaveChangesAsync();

						createData = new FileManagerDirectoryContent
						{
							Name = newFolder.Name,
							Id = newFolder.ItemId.ToString(),
							Size = (long)newFolder.Size,
							IsFile = (bool)newFolder.IsFile,
							DateModified = (DateTime)newFolder.DateModified,
							DateCreated = (DateTime)newFolder.DateCreated,
							Type = newFolder.Type,
							HasChild = (bool)newFolder.HasChild,
							ParentID = newFolder.ParentID.ToString(),
							user_id = user_id,
							url = newFolder.url
						};
						AccessPermission permission = GetPermission(createData.Id, createData.ParentID, createData.Name, createData.IsFile, path);
						createData.permission = permission;


					}
				}
				catch (SqlException ex) { Console.WriteLine(ex.ToString()); }

				createResponse.Files = new FileManagerDirectoryContent[] { createData };
				return createResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				createResponse.Error = error;
				return createResponse;
			}
		}

		// Downloads file(s) and folder(s)
		public FileStreamResult Download(string user_name, string path, string[] names, params FileManagerDirectoryContent[] data)
		{
			List<FileManagerDirectoryContent> files = new List<FileManagerDirectoryContent> { };
			try
			{
				FileStreamResult fileStreamResult = null;
				if (data != null)
				{
					foreach (FileManagerDirectoryContent item in data)
					{
						bool isFile = item.IsFile;
						bool isEsign = item.Type == "Esign";
						AccessPermission permission = GetPermission(item.Id, item.ParentID, item.Name, isFile, path);
						if (permission != null && (!permission.Read || !permission.Download))
						{
							throw new UnauthorizedAccessException("'" + item.Name + "' is not accessible. Access is denied.");
						}

						if (isEsign)
						{
							files.Add(item);
						}
						else if (isFile)
						{
							var path_files = _configuration["Source:Path_Files"] + item.url;
							path_files = path_files.Replace("/", "\\");
							if (File.Exists(path_files))
								files.Add(item);
						}
						else
						{
							files.Add(item);
						}
					}

					if (files.Count == 0)
					{
						return null;
					}
					if (files.Count == 1 && files[0].IsFile)
					{
						var path_files = _configuration["Source:Path_Files"] + files[0].url;
						path_files = path_files.Replace("/", "\\");
						fileStreamResult = new FileStreamResult((new FileStream(path_files, FileMode.Open, FileAccess.Read)), "APPLICATION/octet-stream");
						fileStreamResult.FileDownloadName = files[0].Name.Trim();
					}
					else
					{
						ZipArchiveEntry zipEntry;
						ZipArchive archive;
						var tempPath = Path.Combine(Path.GetTempPath(), "temp.zip");
						using (archive = ZipFile.Open(tempPath, ZipArchiveMode.Update))
						{
							for (var i = 0; i < files.Count; i++)
							{
								bool isFile = files[i].IsFile;
								bool isEsign = files[i].Type == "Esign";
								var Id = Int32.Parse(files[i].Id);
								if (isEsign)
								{
									var path_files = _configuration["Source:Path_Private"].Replace("\\private", "") + files[i].url;
									path_files = path_files.Replace("/", "\\");
									zipEntry = archive.CreateEntryFromFile(path_files, files[i].Name.Trim(), CompressionLevel.Fastest);
								}
								else if (isFile)
								{
									var path_files = _configuration["Source:Path_Files"] + files[i].url;
									path_files = path_files.Replace("/", "\\");
									zipEntry = archive.CreateEntryFromFile(path_files, files[i].Name.Trim(), CompressionLevel.Fastest);
								}
								else
								{
									var item = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
									DownloadFolder(archive, item, user_name);
								}
							}
							archive.Dispose();
							fileStreamResult = new FileStreamResult((new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Delete)), "APPLICATION/octet-stream");
							fileStreamResult.FileDownloadName = files.Count == 1 ? data[0].Name.Trim() + ".zip" : "Files.zip";
							if (File.Exists(Path.Combine(Path.GetTempPath(), "temp.zip")))
								File.Delete(Path.Combine(Path.GetTempPath(), "temp.zip"));
						}
					}
				}
				return fileStreamResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}

		public void DownloadFolder(ZipArchive archive, LibraryModel subFolderName, string user_name)
		{
			LinkedList<LibraryModel> subFolders = new LinkedList<LibraryModel>();
			subFolders.AddLast(subFolderName);

			LinkedList<string> folderPath = new LinkedList<string>();
			folderPath.AddLast(subFolderName.Name.Trim());

			while (subFolders.Any())
			{
				subFolderName = subFolders.First();
				subFolders.RemoveFirst();

				string folderName = folderPath.First();
				folderPath.RemoveFirst();


				ZipArchiveEntry zipEntry = archive.CreateEntry(folderName + "/");


				var downloadReadCommandReader = _context.LibraryModel.Where(d => d.ParentID == subFolderName.ItemId && d.DateDeleted == null).ToList();
				foreach (var downloadEntry in downloadReadCommandReader)
				{
					string fileName = downloadEntry.Name.ToString().Trim();
					int fileId = downloadEntry.ItemId;
					bool isFile = (bool)downloadEntry.IsFile;
					if (isFile)
					{
						var path_files = _configuration["Source:Path_Files"] + downloadEntry.url;
						path_files = path_files.Replace("/", "\\");
						if (System.IO.File.Exists(path_files))
							zipEntry = archive.CreateEntryFromFile(path_files, folderName + "\\" + fileName, CompressionLevel.Fastest);
					}
					else
					{
						folderPath.AddLast(folderName + "/" + fileName);
						subFolders.AddLast(downloadEntry);

					}
				}

			}

		}
		// Calculates the folder size
		public long GetFolderSize(string[] idValue)
		{

			List<string> checkedIDs = new List<string>();
			long sizeValue = 0;
			foreach (var id in idValue)
			{
				checkedIDs.Add(id);
				string removeQuery = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ParentID =" + id + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on p.ParentID = cte.ItemID) select ItemID,Name from cte;";

				var removeCommandReader = _context.FilterIdRaw.FromSqlRaw(removeQuery).AsEnumerable().Select(d => d.ItemId.ToString()).ToList();
				checkedIDs.AddRange(removeCommandReader);
			}
			if (checkedIDs.Count > 0)
			{
				string query = "select Size from " + this.tableName + " where ItemID IN (" + string.Join(", ", checkedIDs.Select(f => "'" + f + "'")) + ")";

				var getDetailsCommandReader = _context.SizeRaw.FromSqlRaw(query).AsEnumerable().Select(d => d.Size).ToList();
				foreach (var size in getDetailsCommandReader)
				{
					sizeValue = sizeValue + size;
				}
			}
			return sizeValue;
		}
		// Gets the details of the file(s) or folder(s)
		public FileManagerResponse Details(string path, string[] names, params FileManagerDirectoryContent[] data)
		{
			bool isVariousFolders = false;
			string previousPath = "";
			string previousName = "";
			FileManagerResponse getDetailResponse = new FileManagerResponse();
			FileDetails detailFiles = new FileDetails();
			try
			{
				if (data.Length == 1)
				{
					if (data[0].Type == "Esign")
					{
						throw new Exception("File Esign!");
					}
					var Id = Int32.Parse(data[0].Id);
					var item_l = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();

					string detailsID = "";
					string detailsParentId = "";
					//string absoluteFilePath = Path.Combine(Path.GetTempPath(), names[0]);

					detailFiles = new FileDetails
					{
						Name = item_l.Name.ToString().Trim(),
						IsFile = (bool)item_l.IsFile,
						Size = (bool)item_l.IsFile ? ByteConversion(long.Parse((item_l.Size).ToString())) : ByteConversion(GetFolderSize(new string[] { data[0].Id })),
						Modified = (DateTime)item_l.DateModified,
						Created = (DateTime)item_l.DateCreated
					};
					detailsID = item_l.ItemId.ToString().Trim();
					detailsParentId = item_l.ParentID.ToString().Trim();
					AccessPermission permission = GetPermission(detailsID, detailsParentId, detailFiles.Name, detailFiles.IsFile, path);
					detailFiles.permission = permission;
					detailFiles.Location = (GetFilterPath(detailsID) + detailFiles.Name).Replace("/", @"\");
				}
				else
				{
					detailFiles = new FileDetails();
					long size = 0;
					var listOfStrings = new List<string>();
					var rootDirectory = "Files";
					foreach (var item in data)
					{
						if (item.Type == "Esign")
						{
							continue;
						}
						detailFiles.Name = previousName == "" ? previousName = item.Name : previousName = previousName + ", " + item.Name;
						previousPath = previousPath == "" ? rootDirectory + item.FilterPath : previousPath;
						if (previousPath == rootDirectory + item.FilterPath && !isVariousFolders)
						{
							previousPath = rootDirectory + item.FilterPath;
							detailFiles.Location = ((item.FilterPath).Replace("/", @"\")).Substring(0, (previousPath.Length - 1));
						}
						else
						{
							isVariousFolders = true;
							detailFiles.Location = "Various Folders";
						}
						if (item.IsFile)
						{
							size = size + item.Size;
						}
						else
						{
							listOfStrings.Add(item.Id);
						}
					}
					var folderValue = listOfStrings.Count > 0 ? GetFolderSize(listOfStrings.ToArray()) : 0;
					detailFiles.Size = ByteConversion((long)(size + folderValue));
					detailFiles.MultipleFiles = true;
				}
				getDetailResponse.Details = detailFiles;
				return getDetailResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				getDetailResponse.Error = error;
				return getDetailResponse;
			}
		}
		// Gets the file type
		public static string GetDefaultExtension(string mimeType)
		{
			string result;
			RegistryKey key;
			object value;
			key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType.Trim(), false);
			value = key != null ? key.GetValue("Extension", null) : null;
			result = value != null ? value.ToString() : string.Empty;
			return result;
		}

		public virtual AccessPermission GetFilePermission(string id, string parentId, string path)
		{
			string fileName = Path.GetFileName(path);
			return GetPermission(id, parentId, fileName, true, path);
		}

		// Returns the image
		public FileStreamResult GetImage(string path, string parentId, string id, bool allowCompress, ImageSize size, params FileManagerDirectoryContent[] data)
		{
			try
			{
				AccessPermission permission = GetFilePermission(id, parentId, path);
				if (permission != null && !permission.Read)
				{
					return null;
				}
				var Id = Int32.Parse(id);
				var library = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
				var path_files = _configuration["Source:Path_Files"] + library.url;
				path_files = path_files.Replace("/", "\\");

				try
				{
					return new FileStreamResult((new FileStream(path_files, FileMode.Open, FileAccess.Read)), "APPLICATION/octet-stream"); ;
				}
				catch (Exception ex) { throw ex; }
			}
			catch (Exception)
			{
				return null;
			}
		}
		// Deletes the file(s) or folder(s)
		public FileManagerResponse Delete(string path, string[] names, params FileManagerDirectoryContent[] data)
		{
			FileManagerResponse remvoeResponse = new FileManagerResponse();
			try
			{
				FileManagerDirectoryContent deletedData = new FileManagerDirectoryContent();
				List<FileManagerDirectoryContent> newData = new List<FileManagerDirectoryContent>();
				foreach (var file in data)
				{
					AccessPermission permission = GetPermission(file.Id, file.ParentID, file.Name, file.IsFile, path);
					if (permission != null && (!permission.Read || !permission.Write))
					{
						accessMessage = permission.Message;
						throw new UnauthorizedAccessException("'" + file.Name + "' is not accessible.  you need permission to perform the write action.");
					}
					var Id = Int32.Parse(file.Id);
					var ParentID = Int32.Parse(file.ParentID);




					int count = _context.LibraryModel.Where(d => d.ParentID == ParentID && d.ItemId != Id && d.DateDeleted == null).Count();
					if (count == 0)
					{
						var parent = _context.LibraryModel.Where(d => d.ItemId == ParentID).FirstOrDefault();
						parent.HasChild = false;
						_context.Update(parent);
						_context.SaveChanges();
					}

					var reader = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
					deletedData = new FileManagerDirectoryContent
					{
						Name = reader.Name,
						Id = reader.ItemId.ToString(),
						Size = (long)reader.Size,
						IsFile = (bool)reader.IsFile,
						DateModified = (DateTime)reader.DateModified,
						DateCreated = (DateTime)reader.DateCreated,
						Type = reader.Type,
						HasChild = (bool)reader.HasChild,
						ParentID = reader.ParentID.ToString(),
						user_id = reader.user_id,
						url = reader.url,
					};
					////Delete Item
					reader.DateDeleted = DateTime.Now;
					_context.Update(reader);
					_context.SaveChanges();
					//// delete child
					string removeQuery = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ParentID =" + Id + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on p.ParentID = cte.ItemID) select ItemID,Name from cte;";
					var deleteFilesId = _context.FilterIdRaw.FromSqlRaw(removeQuery).AsEnumerable().Select(d => d.ItemId).ToList();

					if (deleteFilesId.Count > 0)
					{
						var list = _context.LibraryModel.Where(d => deleteFilesId.Contains(d.ItemId) && d.DateDeleted == null).ToList();
						foreach (var item in list)
						{
							item.DateDeleted = DateTime.Now;
							_context.Update(item);
						}
						_context.SaveChanges();
					}
					newData.Add(deletedData);
					remvoeResponse.Files = newData;
				}


				return remvoeResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage))
				{
					error.Message = accessMessage;
				}
				remvoeResponse.Error = error;
				return remvoeResponse;
			}
		}
		// Uploads the files
		public virtual FileManagerResponse Upload(string path, IList<IFormFile> uploadFiles, string action, string user_id, params FileManagerDirectoryContent[] data)
		{
			FileManagerResponse uploadResponse = new FileManagerResponse();

			try
			{
				AccessPermission permission = GetPermission(data[0].Id, data[0].ParentID, data[0].Name, data[0].IsFile, path);
				if (permission != null && (!permission.Read || !permission.Upload))
				{
					accessMessage = permission.Message;
					throw new UnauthorizedAccessException("'" + data[0].Name + "' is not accessible. You need permission to perform the upload action.");
				}
				//string filterPath = GetFilterPath(data[0].Id).Replace("/", "\\");
				var datetime = DateTime.Now.ToString("yyyy-MM-dd");
				string absolutePath = _configuration["Source:Path_Files"] + "\\Files\\" + datetime;
				if (!Directory.Exists(absolutePath))
				{
					Directory.CreateDirectory(absolutePath);
				}
				//Console.WriteLine(path);
				List<string> existFiles = new List<string>();
				foreach (IFormFile file in uploadFiles)
				{
					if (uploadFiles != null)
					{
						string fileName = Path.GetFileName(file.FileName);
						long size = file.Length;
						string absoluteFilePath = absolutePath + "\\" + fileName;
						string contentType = file.ContentType;
						if (action == "save")
						{
							if (!System.IO.File.Exists(absoluteFilePath))
							{
								using (FileStream fsSource = new FileStream(absoluteFilePath, FileMode.Create))
								{
									file.CopyTo(fsSource);
									fsSource.Close();
									UploadQuery(fileName, contentType, size, absoluteFilePath, data[0].Id, user_id);
								}
							}
							else
							{
								existFiles.Add(fileName);
							}
						}
						//else if (action == "replace")
						//{
						//	FileManagerResponse detailsResponse = this.GetFiles(path, false, data);
						//	if (System.IO.File.Exists(absoluteFilePath))
						//	{
						//		System.IO.File.Delete(absoluteFilePath);

						//		foreach (FileManagerDirectoryContent newData in detailsResponse.Files)
						//		{
						//			string existingFileName = newData.Name.ToString();
						//			if (existingFileName == fileName)
						//			{
						//				this.Delete(path, null, newData);
						//			}
						//		}
						//	}
						//	using (FileStream fsSource = new FileStream(absoluteFilePath, FileMode.Create))
						//	{
						//		file.CopyTo(fsSource);
						//		fsSource.Close();
						//		UploadQuery(fileName, contentType, size, absoluteFilePath, data[0].Id, user_id);
						//	}
						//}
						else if (action == "keepboth" || action == "replace")
						{
							string newAbsoluteFilePath = absoluteFilePath;
							int index = newAbsoluteFilePath.LastIndexOf(".");
							if (index >= 0)
							{
								newAbsoluteFilePath = newAbsoluteFilePath.Substring(0, index);
							}
							int fileCount = 0;
							while (System.IO.File.Exists(newAbsoluteFilePath + (fileCount > 0 ? "(" + fileCount.ToString() + ")" + Path.GetExtension(fileName) : Path.GetExtension(fileName))))
							{
								fileCount++;
							}

							newAbsoluteFilePath = newAbsoluteFilePath + (fileCount > 0 ? "(" + fileCount.ToString() + ")" : "") + Path.GetExtension(fileName);
							string newFileName = Path.GetFileName(newAbsoluteFilePath);
							using (FileStream fsSource = new FileStream(newAbsoluteFilePath, FileMode.Create))
							{
								file.CopyTo(fsSource);
								fsSource.Close();
								UploadQuery(fileName, contentType, size, newAbsoluteFilePath, data[0].Id, user_id);
							}
						}
					}
				}
				if (existFiles.Count != 0)
				{
					ErrorDetails error = new ErrorDetails();
					error.FileExists = existFiles;
					error.Code = "400";
					error.Message = "File Already Exists";
					uploadResponse.Error = error;
				}
				return uploadResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				uploadResponse.Error = error;
				return uploadResponse;
			}

		}
		// Updates the data table after uploading the file
		public void UploadQuery(string fileName, string contentType, long size, string path, string parentId, string user_id)
		{
			path = path.Replace(_configuration["Source:Path_Files"], "").Replace("\\", "/");
			var newFile = new LibraryModel()
			{
				Name = fileName,
				IsFile = true,
				Size = size,
				ParentID = Int32.Parse(parentId),
				MimeType = contentType,
				url = path,
				DateModified = DateTime.Now,
				DateCreated = DateTime.Now,
				HasChild = false,
				IsRoot = false,
				Type = "File",
				user_id = user_id
			};
			_context.Add(newFile);
			_context.SaveChanges();
		}
		// Renames a file or folder
		public async Task<FileManagerResponse> Rename(string path, string name, string newName, bool replace = false, params FileManagerDirectoryContent[] data)
		{
			FileManagerResponse renameResponse = new FileManagerResponse();
			try
			{
				AccessPermission permission = GetPermission(data[0].Id, data[0].ParentID, data[0].Name, data[0].IsFile, path);
				if (permission != null && (!permission.Read || !permission.Write))
				{
					accessMessage = permission.Message;
					throw new UnauthorizedAccessException("'" + data[0].Name + "' is not accessible. You need permission");
				}

				FileManagerDirectoryContent renameData = new FileManagerDirectoryContent();
				var Id = Int32.Parse(data[0].Id);
				var folderFile = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();

				if (data[0].IsFile && folderFile != null && newName.Substring(newName.LastIndexOf(".") + 1) != name.Substring(name.LastIndexOf(".") + 1))
				{
					//sqlConnection.Open();
					string fileExtension = Path.GetExtension(newName);
					string mimeType = "application/unknown";
					Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
					if (regKey != null && regKey.GetValue("Content Type") != null)
						mimeType = regKey.GetValue("Content Type").ToString();
					folderFile.MimeType = mimeType;
				}
				if (folderFile != null)
				{
					//ParentID = folder.ParentID;
					folderFile.DateModified = DateTime.Now;
					folderFile.Name = newName;
					_context.Update(folderFile);
					await _context.SaveChangesAsync();
				}
				renameData = new FileManagerDirectoryContent
				{
					Name = folderFile.Name,
					Id = folderFile.ItemId.ToString(),
					Size = (long)folderFile.Size,
					IsFile = (bool)folderFile.IsFile,
					DateModified = (DateTime)folderFile.DateModified,
					DateCreated = (DateTime)folderFile.DateCreated,
					Type = folderFile.Type,
					HasChild = (bool)folderFile.HasChild,
					ParentID = folderFile.ParentID.ToString(),
					user_id = folderFile.user_id,
					url = folderFile.url,
				};
				renameData.FilterId = GetFilterId(renameData.Id);
				renameData.FilterPath = GetFilterPath(renameData.Id);

				var newData = new FileManagerDirectoryContent[] { renameData };
				renameResponse.Files = newData;
				return renameResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				renameResponse.Error = error;
				return renameResponse;
			}
		}

		public string GetFilterPath(string id)
		{
			try
			{
				List<string> parents = new List<string>();
				var Id = Int32.Parse(id);
				var folder = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
				var idValue = folder.ParentID.ToString().Trim();
				string query = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ItemID =" + idValue + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on cte.ParentID = p.ItemID) select ItemID,Name from cte";
				parents = _context.FilterIdRaw.FromSqlRaw(query).AsEnumerable().Select(d => d.Name.Trim()).ToList();
				return ("/" + (parents.Count > 0 ? (string.Join("/", parents.ToArray().Reverse()) + "/") : ""));
			}
			catch (Exception e)
			{
				return "";
			}
		}

		public string GetFilterId(string id)
		{
			try
			{
				List<int> parents = new List<int>();
				var Id = Int32.Parse(id);
				var folder = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
				var idValue = folder.ParentID.ToString().Trim();
				string query = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ItemID =" + idValue + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on cte.ParentID = p.ItemID) select ItemID,Name from cte";
				parents = _context.FilterIdRaw.FromSqlRaw(query).AsEnumerable().Select(d => (int)d.ItemId).ToList();
				return (string.Join("/", parents.ToArray().Reverse()) + "/");
			}
			catch (Exception e)
			{
				return "";
			}
		}
		// Search for file(s) or folder(s)
		public FileManagerResponse Search(string path, string searchString, bool showHiddenItems, bool caseSensitive, params FileManagerDirectoryContent[] data)
		{
			FileManagerResponse searchResponse = new FileManagerResponse();
			try
			{
				if (path == null) { path = string.Empty; };
				var searchWord = searchString.Replace("*", "").Replace("%", "");
				FileManagerDirectoryContent searchData;
				FileManagerDirectoryContent cwd = data[0];
				AccessPermission permission = GetPermission(data[0].Id, data[0].ParentID, cwd.Name, cwd.IsFile, path);
				cwd.permission = permission;
				if (cwd.permission != null && !cwd.permission.Read)
				{
					accessMessage = cwd.permission.Message;
					throw new UnauthorizedAccessException("'" + cwd.Name + "' is not accessible. You need permission to perform the read action.");
				}
				searchResponse.CWD = cwd;
				List<FileManagerDirectoryContent> foundFiles = new List<FileManagerDirectoryContent>();


				string removeQuery = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ParentID =" + data[0].Id + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on p.ParentID = cte.ItemID) select ItemID,Name from cte;";

				var availableFiles = _context.FilterIdRaw.FromSqlRaw(removeQuery).AsEnumerable().Select(d => d.ItemId).ToList();

				if (availableFiles.Count > 0)
				{
					var reader = _context.LibraryModel.Where(d => d.Name.Contains(searchWord) && availableFiles.Contains(d.ItemId) && d.ParentID != 0 && d.DateDeleted == null).ToList();
					foreach (var lib in reader)
					{
						searchData = new FileManagerDirectoryContent
						{
							Name = lib.Name,
							Id = lib.ItemId.ToString(),
							Size = (long)lib.Size,
							IsFile = (bool)lib.IsFile,
							DateModified = (DateTime)lib.DateModified,
							DateCreated = (DateTime)lib.DateCreated,
							Type = GetDefaultExtension(lib.MimeType),
							HasChild = (bool)lib.HasChild,
							ParentID = lib.ParentID.ToString(),
							user_id = lib.user_id,
							url = lib.url,
						};
						AccessPermission searchPermission = GetPermission(searchData.Id, searchData.ParentID, searchData.Name, searchData.IsFile, path);
						searchData.permission = searchPermission;
						searchData.FilterPath = GetFilterPath(searchData.Id);
						searchData.FilterId = GetFilterId(searchData.Id);
						var hasPermission = parentsHavePermission(searchData);
						if (hasPermission)
							foundFiles.Add(searchData);
					}
				}
				searchResponse.Files = (IEnumerable<FileManagerDirectoryContent>)foundFiles;
				return searchResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				searchResponse.Error = error;
				return searchResponse;
			}
		}
		protected virtual bool parentsHavePermission(FileManagerDirectoryContent fileDetails)
		{
			String[] parentPath = fileDetails.FilterId.Split('/');
			bool hasPermission = true;
			for (int i = 0; i <= parentPath.Length - 3; i++)
			{
				AccessPermission pathPermission = GetPermission(fileDetails.ParentID, parentPath[i], fileDetails.Name, false, fileDetails.FilterId);
				if (pathPermission == null)
				{
					break;
				}
				else if (pathPermission != null && !pathPermission.Read)
				{
					hasPermission = false;
					break;
				}
			}
			return hasPermission;
		}

		// Copies the selected folder
		public void CopyFolderFiles(List<int> folderId, List<int> newTargetId, string user_id)
		{
			List<int> fromFoldersId = new List<int>();
			List<int> toFoldersId = new List<int>();
			for (var i = 0; i < folderId.Count(); i++)
			{
				///COPY file và FOLDER
				var all = _context.LibraryModel.Where(d => d.ParentID == folderId[i] && d.DateDeleted == null).ToList();
				foreach (var item in all)
				{
					item.ItemId = 0;
					item.DateCreated = DateTime.Now;
					item.DateModified = DateTime.Now;
					item.ParentID = newTargetId[i];
					item.user_id = user_id;
					_context.Add(item);
					_context.SaveChanges();

				}

				///COPY FOLDER
				fromFoldersId = _context.LibraryModel.Where(d => d.ParentID == folderId[i] && d.Type == "Folder" && d.DateDeleted == null).Select(d => d.ItemId).ToList();

				////TO FOLDER 
				toFoldersId = _context.LibraryModel.Where(d => d.ParentID == newTargetId[i] && d.Type == "Folder" && d.DateDeleted == null).Select(d => d.ItemId).ToList();

			}
			if (fromFoldersId.Count > 0)
				CopyFolderFiles(fromFoldersId, toFoldersId, user_id);
		}
		// Copies the selected file(s) or folder(s)
		public FileManagerResponse Copy(string path, string targetPath, string[] names, string[] replacedItemNames, string user_id, FileManagerDirectoryContent targetData, params FileManagerDirectoryContent[] data)
		{
			List<FileManagerDirectoryContent> files = new List<FileManagerDirectoryContent>();
			FileManagerResponse copyResponse = new FileManagerResponse();
			try
			{
				/////CHECK PERMISSION
				foreach (var item in data)
				{
					if (item.Type.ToLower() == "esign")
					{
						throw new UnauthorizedAccessException("Không cho phép sao chép File Esign!");
					}
					AccessPermission permission = GetPermission(item.Id, item.ParentID, item.Name, item.IsFile, path);
					if (permission != null && (!permission.Copy || !permission.WriteContents))
					{
						accessMessage = permission.Message;
						throw new UnauthorizedAccessException("'" + item.Name + "' is not accessible. You need permission to perform the write action.");
					}

					string checkingQuery = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ParentID =" + item.Id + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on p.ParentID = cte.ItemID) select ItemID,Name from cte;";

					var checkingId = _context.FilterIdRaw.FromSqlRaw(checkingQuery).AsEnumerable().Select(d => d.ItemId.ToString()).ToList();

					if (checkingId.IndexOf(targetData.Id) != -1)
					{
						ErrorDetails error = new ErrorDetails();
						error.Code = "400";
						error.Message = "The destination folder is the subfolder of the source folder.";
						copyResponse.Error = error;
						return copyResponse;
					}
				}

				foreach (var item in data)
				{
					List<int> foldersId = new List<int>();
					List<int> toFoldersId = new List<int>();
					var Id = Int32.Parse(item.Id);
					var targetId = Int32.Parse(targetData.Id);
					///COPY file và FOLDER
					var fileFolder = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
					fileFolder.ItemId = 0;
					fileFolder.DateCreated = DateTime.Now;
					fileFolder.DateModified = DateTime.Now;
					fileFolder.ParentID = targetId;
					fileFolder.user_id = user_id;
					_context.Add(fileFolder);
					_context.SaveChanges();

					//fileFolder.FilterPath = GetFilterId(fileFolder.ItemId.ToString());
					//_context.Update(fileFolder);
					//_context.Save();

					if (fileFolder.Type == "Folder")
					{
						//CHECK PARENT
						var parentFolder = _context.LibraryModel.Where(d => d.ItemId == targetId).FirstOrDefault();
						parentFolder.HasChild = true;
						_context.Update(parentFolder);
						_context.SaveChanges();

						foldersId.Add(Id);
						toFoldersId.Add(fileFolder.ItemId);
					}

					var copyFiles = new FileManagerDirectoryContent
					{
						Name = fileFolder.Name,
						Id = fileFolder.ItemId.ToString(),
						Size = (long)fileFolder.Size,
						IsFile = (bool)fileFolder.IsFile,
						DateModified = (DateTime)fileFolder.DateModified,
						DateCreated = (DateTime)fileFolder.DateCreated,
						Type = GetDefaultExtension(fileFolder.MimeType),
						HasChild = (bool)fileFolder.HasChild,
						ParentID = fileFolder.ParentID.ToString(),
						user_id = fileFolder.user_id,
						url = fileFolder.url
					};
					AccessPermission permission1 = GetPermission(copyFiles.Id, copyFiles.ParentID, copyFiles.Name, copyFiles.IsFile, path);
					copyFiles.permission = permission1;
					copyFiles.FilterId = GetFilterId(copyFiles.Id);
					copyFiles.FilterPath = GetFilterPath(copyFiles.Id);
					files.Add(copyFiles);

					if (foldersId.Count > 0)
						CopyFolderFiles(foldersId, toFoldersId, user_id);

				}
				copyResponse.Files = files;
				return copyResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = error.Message.Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				copyResponse.Error = error;
				return copyResponse;
			}
		}

		public virtual string[] GetFolderDetails(string path)
		{
			string[] str_array = path.Split('/'), fileDetails = new string[2];
			string parentPath = "";
			for (int i = 0; i < str_array.Length - 2; i++)
			{
				parentPath += str_array[i] + "/";
			}
			fileDetails[0] = parentPath;
			fileDetails[1] = str_array[str_array.Length - 2];
			return fileDetails;
		}

		// Moves the selected file(s) or folder(s) to target path
		public FileManagerResponse Move(string path, string targetPath, string[] names, string[] replacedItemNames, FileManagerDirectoryContent targetData, params FileManagerDirectoryContent[] data)
		{
			List<FileManagerDirectoryContent> files = new List<FileManagerDirectoryContent>();
			FileManagerResponse moveResponse = new FileManagerResponse();
			try
			{
				/////CHECK PERMISSION
				foreach (var item in data)
				{
					if (item.Type.ToLower() == "esign")
					{
						throw new UnauthorizedAccessException("Không cho phép sao chép File Esign!");
					}
					AccessPermission permission = GetPermission(item.Id, item.ParentID, item.Name, item.IsFile, path);
					if (permission != null && (!permission.Copy || !permission.Write))
					{
						accessMessage = permission.Message;
						throw new UnauthorizedAccessException("'" + item.Name + "' is not accessible. You need permission to perform the write action.");
					}

					string checkingQuery = "with cte as (select ItemID, Name, ParentID from " + this.tableName + " where ParentID =" + item.Id + " union all select p.ItemID, p.Name, p.ParentID from " + this.tableName + " p inner join cte on p.ParentID = cte.ItemID) select ItemID,Name from cte;";

					var checkingId = _context.FilterIdRaw.FromSqlRaw(checkingQuery).AsEnumerable().Select(d => d.ItemId.ToString()).ToList();

					if (checkingId.IndexOf(targetData.Id) != -1)
					{
						ErrorDetails error = new ErrorDetails();
						error.Code = "400";
						error.Message = "The destination folder is the subfolder of the source folder.";
						moveResponse.Error = error;
						return moveResponse;
					}
				}

				////START MOVE
				foreach (var item in data)
				{
					var parentID = Int32.Parse(item.ParentID);
					var Id = Int32.Parse(item.Id);
					var target_Id = Int32.Parse(targetData.Id);
					var fileFolder = _context.LibraryModel.Where(d => d.ItemId == Id).FirstOrDefault();
					fileFolder.ParentID = target_Id;
					_context.Update(fileFolder);
					_context.SaveChanges();


					if (fileFolder.Type == "Folder")
					{
						//CHECK PARENT TARGET
						var parentFolder = _context.LibraryModel.Where(d => d.ItemId == target_Id).FirstOrDefault();
						parentFolder.HasChild = true;
						_context.Update(parentFolder);
						_context.SaveChanges();

						///CHECK PARENT SOURCE
						var countFolder = _context.LibraryModel.Where(d => d.ParentID == parentID && d.Type == "Folder" && d.DateDeleted == null).Count();
						if (countFolder == 0)
						{
							var parentFolderSource = _context.LibraryModel.Where(d => d.ItemId == parentID).FirstOrDefault();
							parentFolderSource.HasChild = false;
							_context.Update(parentFolderSource);
							_context.SaveChanges();
						}
					}


					var moveFiles = new FileManagerDirectoryContent
					{
						Name = fileFolder.Name,
						Id = fileFolder.ItemId.ToString(),
						Size = (long)fileFolder.Size,
						IsFile = (bool)fileFolder.IsFile,
						DateModified = (DateTime)fileFolder.DateModified,
						DateCreated = (DateTime)fileFolder.DateCreated,
						Type = GetDefaultExtension(fileFolder.MimeType),
						HasChild = (bool)fileFolder.HasChild,
						ParentID = fileFolder.ParentID.ToString(),
						user_id = fileFolder.user_id,
						url = fileFolder.url,
					};
					AccessPermission permission1 = GetPermission(moveFiles.Id, moveFiles.ParentID, moveFiles.Name, moveFiles.IsFile, path);
					moveFiles.permission = permission1;
					moveFiles.FilterId = GetFilterId(moveFiles.Id);
					moveFiles.FilterPath = GetFilterPath(moveFiles.Id);
					files.Add(moveFiles);
				}
				moveResponse.Files = files;
				return moveResponse;
			}
			catch (Exception e)
			{
				ErrorDetails error = new ErrorDetails();
				error.Message = e.Message.ToString();
				error.Code = e.Message.ToString().Contains("is not accessible. You need permission") ? "401" : "417";
				if ((error.Code == "401") && !string.IsNullOrEmpty(accessMessage)) { error.Message = accessMessage; }
				moveResponse.Error = error;
				return moveResponse;
			}
		}

		// Converts the byte value to the appropriate size value
		public String ByteConversion(long fileSize)
		{
			try
			{
				string[] index = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; // Longs run out around EB
				if (fileSize == 0) return "0 " + index[0];
				int loc = Convert.ToInt32(Math.Floor(Math.Log(Math.Abs(fileSize), 1024)));
				return (Math.Sign(fileSize) * Math.Round(Math.Abs(fileSize) / Math.Pow(1024, loc), 1)).ToString() + " " + index[loc];
			}
			catch (Exception e) { throw e; }
		}

		public string ToCamelCase(FileManagerResponse userData)
		{
			return JsonConvert.SerializeObject(userData, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });
		}
	}

}
