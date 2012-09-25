#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics;
using System.IO;

namespace MavenWrapper {
	internal class Program {
		private static void Main(string[] args) {
			try
			{
				var process = Process.Start("clean.bat");
				process.WaitForExit();
			}
			catch {
			}

			var arg = args.Length > 0 ? string.Join(" ", args) : "test";
			var mvnPath = Environment.GetEnvironmentVariable("M2_HOME") ??
				@"C:\Users\exKAZUu\Dropbox\Private\Tools\Development\Maven3";
			var info = new ProcessStartInfo {
					FileName = Path.Combine(mvnPath, "bin", "mvn.bat"),
					Arguments = arg,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = false,
			};
			using (var fs = File.OpenWrite("mvn_out.log"))
			using (var writer = new StreamWriter(fs))
			using (var fsErr = File.OpenWrite("mvn_err.log"))
			using (var writerErr = new StreamWriter(fsErr))
			using (var process = Process.Start(info)) {
				var succeeded = false;
				process.OutputDataReceived += (sender, eventArgs) => {
					var line = eventArgs.Data;
					if (!string.IsNullOrEmpty(line)) {
						writer.WriteLine(line);
						succeeded |= line.Contains("BUILD SUCCESS");
					}
				};
				process.ErrorDataReceived += (sender, eventArgs) => {
					var line = eventArgs.Data;
					if (!string.IsNullOrEmpty(line)) {
						writerErr.WriteLine(line);
					}
				};
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (succeeded) {
					Console.WriteLine("BUILD SUCCESSFUL");
				} else {
					Console.Error.WriteLine("BUILD FAILED");
				}
			}
		}
	}
}