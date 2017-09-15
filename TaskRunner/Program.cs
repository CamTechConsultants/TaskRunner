/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace TaskRunner
{
	class Program
	{
		private const string RegistryKeyPath = @"SOFTWARE\CTC\TaskRunner";

		static int Main(string[] args)
		{
			try
			{
				var switches = new Switches(args);
				switches.Parse();

				if (switches.ConfigureEmail)
					WriteRegistrySettings(switches.EmailSettings);

				if (!switches.TargetCommandLine.Any())
					return 0;

				var emailSettings = GetEmailSettings(switches.EmailSettings);

				var result = RunProcess(switches.TargetCommandLine);

				if (result.ExitCode != 0 || result.Output.Length > 0)
					SendEmail(emailSettings, result, switches.TargetCommandLine);

				return result.ExitCode;
			}
			catch (CommandLineArgumentException clae)
			{
				Console.Error.WriteLine($"{clae.Message}");
				return 1;
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"An unexpected exception occurred: {e}");
				return 1;
			}
		}

		private static void SendEmail(EmailSettings emailSettings, RunResult result, IEnumerable<string> args)
		{
			var cmdLine = MakeCommandLine();
			var status = (result.ExitCode == 0) ? "succeeded" : "failed";
			var computerName = Environment.MachineName;
			var subject = $"[{computerName}] Task {status} : {cmdLine}";

			var bodyLines = new List<string>()
			{
				$"Computer: {computerName}",
				$"Username: {Environment.UserDomainName}\\{Environment.UserName}",
				$"Command line: {cmdLine}",
			};

			if (result.Output.Length > 0)
			{
				const string sep = "------------------------------------------------------------------------";
				bodyLines.AddRange(new[] { "", "Output:", sep, result.Output.Trim(), sep });
			}

			var body = String.Join("\r\n", bodyLines);
			var client = new SmtpClient(emailSettings.Host);

			var msg = new MailMessage()
			{
				From = emailSettings.From,
				Subject = subject,
				Body = body
			};
			msg.To.Add(emailSettings.To);
			client.Send(msg);

			string MakeCommandLine()
			{
				return String.Join(" ", args.Select(a => a.Contains(" ") ? "\"" + a + "\"" : a));
			}
		}

		private static RunResult RunProcess(IEnumerable<string> args)
		{
			var process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = $"/C \"{EscapeArguments()}\"",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
				},
			};

			var output = new StringBuilder();
			process.OutputDataReceived += (_, e) => HandleOutput(e.Data);
			process.ErrorDataReceived += (_, e) => HandleOutput(e.Data);

			try
			{
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();

				return new RunResult(process.ExitCode, output.ToString());
			}
			catch (Win32Exception w32e)
			{
				return new RunResult(1, $"Failed to start the task: {w32e.Message}");
			}


			void HandleOutput(string text)
			{
				if (text != null)
				{
					lock (output)
						output.AppendLine(text);
				}
			};

			string EscapeArguments()
			{
				var buf = new StringBuilder();

				bool first = true;
				foreach (var arg in args)
				{
					if (first)
						first = false;
					else
						buf.Append(" ");

					if (String.IsNullOrEmpty(arg))
					{
						buf.Append("\"\"");
					}
					else
					{
						// https://stackoverflow.com/a/12364234/214776
						string escaped = Regex.Replace(arg, @"(\\*)" + "\"", @"$1\$0");
						escaped = Regex.Replace(escaped, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"", RegexOptions.Singleline);
						buf.Append(escaped);
					}
				}

				return buf.ToString();
			}
		}

		private static EmailSettings GetEmailSettings(EmailSettings cmdLineSettings)
		{
			var systemSettings = GetSystemSettings();
			var currentUserRegistrySettings = GetRegistrySettings(Registry.CurrentUser);
			var localMachineRegistrySettings = GetRegistrySettings(Registry.LocalMachine);

			var settings = systemSettings
					.MergeIn(localMachineRegistrySettings)
					.MergeIn(currentUserRegistrySettings)
					.MergeIn(cmdLineSettings);

			ValidateEmailSettings(settings);
			return settings;
		}

		private static EmailSettings GetSystemSettings()
		{
			var hostName = Dns.GetHostEntry("localhost").HostName;
			var from = new MailAddress($"{Environment.UserName}@{hostName}", Environment.UserName);
			return new EmailSettings(from: from);
		}

		private static EmailSettings GetRegistrySettings(RegistryKey root)
		{
			string host = null;
			MailAddress from = null;
			MailAddress to = null;

			try
			{
				using (var key = root.OpenSubKey(RegistryKeyPath))
				{
					if (key != null)
					{
						host = key.GetValue("Host", null) as string;

						if (key.GetValue("From", null) is string fromString)
							Email.TryParseAddress(fromString, out from);

						if (key.GetValue("To", null) is string toString)
							Email.TryParseAddress(toString, out to);
					}
				}
			}
			catch (System.Security.SecurityException)
			{
			}
			catch (System.IO.IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}

			return new EmailSettings(host, from, to);
		}

		private static void WriteRegistrySettings(EmailSettings settings)
		{
			using (var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
			{
				if (settings.Host != null)
					key.SetValue("Host", settings.Host, RegistryValueKind.String);
				if (settings.From != null)
					key.SetValue("From", settings.From.ToString(), RegistryValueKind.String);
				if (settings.To != null)
					key.SetValue("To", settings.To.ToString(), RegistryValueKind.String);
			}
		}

		private static void ValidateEmailSettings(EmailSettings emailSettings)
		{
			if (!emailSettings.IsValid)
				throw new CommandLineArgumentException("Not all e-mail settings have been provided");
		}
	}
}