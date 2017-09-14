/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */


using System;

namespace TaskRunner
{
	struct RunResult
	{
		public int ExitCode { get; }

		public string Output { get; }

		public RunResult(int exitCode, string output)
		{
			ExitCode = exitCode;
			Output = output;
		}

		public override string ToString()
		{
			if (ExitCode == 0)
				return String.IsNullOrEmpty(Output) ? "Passed" : "Passed with output";
			else
				return $"Exit code {ExitCode}";
		}
	}
}