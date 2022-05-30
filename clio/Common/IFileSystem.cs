﻿using System.Collections.Generic;
using System.IO;

namespace Clio.Common
{

	#region Interface: IFileSystem

	public interface IFileSystem
	{

		#region Methods: Public

		void CheckOrDeleteExistsFile(string filePath, bool delete);
		void CopyFiles(IEnumerable<string> filesPaths, string destinationDirectory, bool overwrite);
		bool DeleteFile(string filePath);
		bool DeleteFileIfExists(string filePath);
		bool ExistsFile(string filePath);
		string ExtractNameFromPath(string filePath);
		string[] GetFiles(string filePath);
		string[] GetFiles(string filePath, string searchPattern, SearchOption searchOption);
		bool IsReadOnlyFile(string filePath);
		void ResetFileReadOnlyAttribute(string filePath);
		void WriteAllTextToFile(string filePath, string contents);
		void CheckOrClearExistsDirectory(string directoryPath, bool overwrite);
		void CheckOrOverwriteExistsDirectory(string directoryPath, bool overwrite);
		void ClearDirectory(string directoryPath);
		void CopyDirectory(string source, string destination, bool overwrite);
		DirectoryInfo CreateDirectory(string directoryPath);
		void DeleteDirectory(string directoryPath);
		void DeleteDirectory(string directoryPath, bool recursive);
		void DeleteDirectoryIfExists(string directoryPath);
		bool ExistsDirectory(string directoryPath);
		string GetCurrentDirectoryIfEmpty(string directoryPath);
		string GetDestinationFileDirectory(string filePath, string destinationPath);
		string[] GetDirectories(string directoryPath);
		void OverwriteExistsDirectory(string directoryPath);
		void SafeDeleteDirectory(string directoryPath);
		string ConvertToRelativePath(string path, string rootDirectoryPath);

		#endregion

	}

	#endregion

}