﻿/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace TaskRunner
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				int targetArg = 0;
				var emailSettings = ParseSwitches(args, ref targetArg);
				ValidateEmailSettings(emailSettings);

				if (targetArg >= args.Length || targetArg == 0)
					throw new CommandLineArgumentException("No program to run was specified");

				var targetArgs = args.Skip(targetArg);
				var result = RunProcess(targetArgs);

				if (result.ExitCode != 0 || result.Output.Length > 0)
					SendEmail(emailSettings, result, targetArgs);

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
					FileName = args.First(),
					Arguments = EscapeArguments(),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
				},
			};

			var output = new StringBuilder();
			process.OutputDataReceived += (_, e) => HandleOutput(e.Data);
			process.ErrorDataReceived += (_, e) => HandleOutput(e.Data);

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			return new RunResult(process.ExitCode, output.ToString());


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
				foreach (var arg in args.Skip(1))
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

		private static EmailSettings ParseSwitches(string[] args, ref int nextArgIndex)
		{
			string host = null;
			MailAddress from = null;
			MailAddress to = null;

			for (int i = nextArgIndex; i < args.Length; i++)
			{
				string arg = args[i];

				if (!arg.StartsWith("-"))
				{
					throw new CommandLineArgumentException($"Unrecognised switch: \"{arg}\"");
				}
				else if (arg == "--")
				{
					nextArgIndex = i + 1;
					break;
				}

				if (i + 1 >= args.Length)
					throw new CommandLineArgumentException($"Missing argument to {arg} switch");

				string nextArg = args[++i];
				switch (arg)
				{
					case "-h":
					case "--host":
						host = nextArg;
						break;
					case "-f":
					case "--from":
						from = ParseAddress(nextArg);
						break;
					case "-t":
					case "--to":
						to = ParseAddress(nextArg);
						break;
				}
			}

			return new EmailSettings(host: host, from: from, to: to);


			MailAddress ParseAddress(string address)
			{
				try
				{
					return new MailAddress(address);
				}
				catch (FormatException)
				{
					throw new CommandLineArgumentException($"Invalid e-mail address: \"{address}\"");
				}
			}
		}

		private static void ValidateEmailSettings(EmailSettings emailSettings)
		{
			if (!emailSettings.IsValid)
				throw new CommandLineArgumentException("Not all e-mail settings have been provided");
		}
	}
}