﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Json;
using Clio.Command.ApplicationCommand;
using NUnit.Framework;

namespace Clio.Tests.Command.ApplicationCommand
{

	internal class SetApplicationVersionCommandTest: BaseCommandTests<SetApplicationVersionOption>
	{

		private static string mockWorspacePath = Path.Combine("C:", "MockWorkspaceFolder");
		private static string appDescriptorJsonPath = Path.Combine(mockWorspacePath, "packages", "IFrameSample", "Files", "app-descriptor.json");

		private static MockFileSystem CreateFs(string filePath) {
			string originClioSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			string appDescriptorExamplesDescriptorPath = Path.Combine(originClioSourcePath, "Examples", "AppDescriptors", filePath);

			return new MockFileSystem(new Dictionary<string, MockFileData> {
				{
					appDescriptorJsonPath,
					new MockFileData(File.ReadAllText(appDescriptorExamplesDescriptorPath))
				}
			});
		}

		private static MockFileSystem CreateFs(Dictionary<string, string> appDescriptors) {
			string originClioSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			MockFileSystem mockFileSystem = new MockFileSystem();
			foreach (var appDescriptor in appDescriptors) {
				string appDescriptorExamplesDescriptorPath = Path.Combine(originClioSourcePath, "Examples", "AppDescriptors", appDescriptor.Value);
				string mockAppDescriptorJsonPath = Path.Combine(mockWorspacePath, "packages", appDescriptor.Key, "Files", "app-descriptor.json");
				mockFileSystem.AddFile(mockAppDescriptorJsonPath, new MockFileData(File.ReadAllText(appDescriptorExamplesDescriptorPath)));
			}
			return mockFileSystem;
		}

		private MockFileSystem _fileSystem;

		[TestCase("app-descriptor_v.json")]
		[TestCase("app-descriptor_wv.json")]
		[TestCase("app-descriptor_dv.json")]
		public void SetVersion_WhenWorkspaceContainsOneApplication(string descriptorPath) {
			_fileSystem = CreateFs(descriptorPath);
			string expectedVersion = "8.1.1";
			var command = new SetApplicationVersionCommand(_fileSystem);
			string worspaceFolderPath = mockWorspacePath;
			command.Execute(new SetApplicationVersionOption() {
				Version = expectedVersion, WorspaceFolderPath = worspaceFolderPath
			});
			var objectJson = JsonObject.Parse(_fileSystem.File.ReadAllText(appDescriptorJsonPath));
			string actualVersion = objectJson["Version"];
			Assert.True(_fileSystem.FileExists(appDescriptorJsonPath));
			Assert.AreEqual(expectedVersion, actualVersion);
			Assert.Greater(20, _fileSystem.File.ReadAllLines(appDescriptorJsonPath).Length);
		}

		[Test]
		public void SetVersion_ThrowException_WhenWorkspaceContainsMoreThanOneApplication() {
			Dictionary<string, string> appDescriptions = new Dictionary<string, string>();
			appDescriptions.Add("Package1", "app1-app-descriptor.json");
			appDescriptions.Add("Package2", "app2-app-descriptor.json");
			_fileSystem = CreateFs(appDescriptions);
			string expectedVersion = "8.1.1";
			var command = new SetApplicationVersionCommand(_fileSystem);
			string worspaceFolderPath = mockWorspacePath;
			var exception = Assert.Throws<Exception>( () => command.Execute(new SetApplicationVersionOption() { 
				Version = expectedVersion, WorspaceFolderPath = worspaceFolderPath }));
			Assert.True(exception.Message.Contains("Package1"));
			Assert.True(exception.Message.Contains("Package2"));
		}

		[Test]
		public void SetVersion_ThrowExceptionWhenAplicationExtendedAndPackageNotDefined() {
			Dictionary<string, string> appDescriptions = new Dictionary<string, string>();
			appDescriptions.Add("Package1", "app1-app-descriptor.json");
			appDescriptions.Add("Package2", "app1-ext-app-descriptor.json");
			_fileSystem = CreateFs(appDescriptions);
			string expectedVersion = "8.1.1";
			var command = new SetApplicationVersionCommand(_fileSystem);
			string worspaceFolderPath = mockWorspacePath;
			var exception = Assert.Throws<Exception>(() => command.Execute(new SetApplicationVersionOption() { 
				Version = expectedVersion, WorspaceFolderPath = worspaceFolderPath }));
			Assert.True(exception.Message.Contains("Package1"));
			Assert.True(exception.Message.Contains("Package2"));
		}

		[Test]
		public void SetVersion_WhenAplicationExtendedAndPackageDefined() {
			Dictionary<string, string> appDescriptions = new Dictionary<string, string>();
			string extendPackageName = "Package2";
			appDescriptions.Add("Package1", "app1-app-descriptor.json");
			appDescriptions.Add(extendPackageName, "app1-ext-app-descriptor.json");
			_fileSystem = CreateFs(appDescriptions);
			string expectedVersion = "8.1.1";
			var command = new SetApplicationVersionCommand(_fileSystem);
			string worspaceFolderPath = mockWorspacePath;
			command.Execute(new SetApplicationVersionOption() {
				Version = expectedVersion,
				WorspaceFolderPath = worspaceFolderPath,
				PackageName = extendPackageName
			});
		}

	}
}