
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.Base.SQLFileProvider;
using Vue.Data;
using Vue.Models;

namespace workflow.Areas.V1.Controllers
{
	public class SQLProviderController : BaseController
	{
		SQLFileProvider operation;
		private UserManager<UserModel> UserManager;
		private IConfiguration _configuration;
		public SQLProviderController(ItContext context, UserManager<UserModel> UserMgr, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			UserManager = UserMgr;
			operation = new SQLFileProvider(context, configuration);
			string tableName = "Library";
			string rootFolderID = "0";

			// Validate table name
			if (!Regex.IsMatch(tableName, "^[a-zA-Z0-9_]*$"))
			{
				throw new ArgumentException("Invalid table name");
			}

			// Validate root folder ID
			if (!Regex.IsMatch(rootFolderID, "^[0-9]*$"))
			{
				throw new ArgumentException("Invalid root folder ID");
			}

			//To configure the database connection, set the connection name, table name and root folder ID value by passing these values to the SetSQLConnection method.
			operation.SetSQLConnection(tableName, rootFolderID);
		}
		//[Route("SQLFileOperations")]
		public async Task<object> SQLFileOperations([FromBody] FileManagerDirectoryContent args)
		{

			var user_id = UserManager.GetUserId(this.User);
			//var owner = _context.LibraryModel.Where(d => d.user_id == user_id && d.Type == "Folder").OrderBy(d => d.FilterPath).ToList();

			//permission.Add(new LibraryPermissionModel
			//	{
			//		path = args.Path,
			//		user_id = user_id,
			//		permission = "Owner",
			//		item_id = Int32.Parse(args.Data[0].Id)
			//	});

			var rules = this.operation.GetRules(user_id, args.custom);
			this.operation.SetRules(rules);
			if ((args.Action == "delete" || args.Action == "rename") && ((args.TargetPath == null) && (args.Path == "")))
			{
				FileManagerResponse response = new FileManagerResponse();
				response.Error = new ErrorDetails { Code = "403", Message = "Restricted to modify the root folder." };
				return operation.ToCamelCase(response);
			}

			switch (args.Action)
			{
				case "read":
					// Reads the file(s) or folder(s) from the given path.
					return operation.ToCamelCase(operation.GetFiles(args.Path, false, args.Data));
				case "delete":
					// Deletes the selected file(s) or folder(s) from the given path.
					return operation.ToCamelCase(operation.Delete(args.Path, args.Names, args.Data));
				case "details":
					// Gets the details of the selected file(s) or folder(s).
					return operation.ToCamelCase(operation.Details(args.Path, args.Names, args.Data));
				case "create":
					// Creates a nYou need permission to perform the read action.ew folder in a given path.
					return operation.ToCamelCase(await operation.Create(args.Path, args.Name, user_id, args.Data));
				case "search":
					// Gets the list of file(s) or folder(s) from a given path based on the searched key string.
					return operation.ToCamelCase(operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive, args.Data));
				case "rename":
					// Renames a file or folder.
					return operation.ToCamelCase(await operation.Rename(args.Path, args.Name, args.NewName, false, args.Data));
				case "move":
					// Cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
					return operation.ToCamelCase(operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData, args.Data));
				case "copy":
					// Copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
					return operation.ToCamelCase(operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, user_id, args.TargetData, args.Data));
			}
			return null;
		}

		// Uploads the file(s) into a specified path
		//[Route("SQLUpload")]
		public IActionResult SQLUpload(string path, IList<IFormFile> uploadFiles, string action, string data)
		{
			var user_id = UserManager.GetUserId(this.User);
			FileManagerResponse uploadResponse;
			FileManagerDirectoryContent[] dataObject = new FileManagerDirectoryContent[1];
			dataObject[0] = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(data);
			uploadResponse = operation.Upload(path, uploadFiles, action, user_id, dataObject);
			if (uploadResponse.Error != null)
			{
				Response.Clear();
				Response.ContentType = "application/json; charset=utf-8";
				Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
				Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
			}
			return Content("");
		}

		// Downloads the selected file(s) and folder(s)
		//[Route("SQLDownload")]
		public async Task<IActionResult> SQLDownload(string downloadInput)
		{

			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			var user = await UserManager.GetUserAsync(currentUser);
			var user_name = user.FullName;
			FileManagerDirectoryContent args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);
			args.Path = (args.Path);
			return operation.Download(user_name, args.Path, args.Names, args.Data);
		}

		// Gets the image(s) from the given path
		//[Route("SQLGetImage")]
		public IActionResult SQLGetImage(FileManagerDirectoryContent args)
		{
			return operation.GetImage(args.Path, args.ParentID, args.Id, true, null, args.Data);
		}
		public async Task<IActionResult> viewLibrary(int id)
		{
			var library = _context.LibraryModel.Where(d => d.ItemId == id).FirstOrDefault();
			if (library == null)
				return Ok();
			var isFile = library.IsFile;
			//var type = library.Type;
			if (isFile == false)
				return Ok();

			var path_files = _configuration["Source:Path_Files"] + library.url;
			path_files = path_files.Replace("/", "\\");
			if (!System.IO.File.Exists(path_files))
				return Ok();


			var myfile = System.IO.File.ReadAllBytes(path_files);
			return new FileContentResult(myfile, "APPLICATION/octet-stream")
			{
				FileDownloadName = library.Name
			};
		}
	}


}
