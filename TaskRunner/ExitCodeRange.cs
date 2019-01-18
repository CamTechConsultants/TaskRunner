/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */


using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskRunner
{
	struct ExitCodeRange : IEquatable<ExitCodeRange>
	{
		/// <summary>
		/// The lower value in the range (inclusive).
		/// </summary>
		public int From { get; }

		/// <summary>
		/// The upper value in the range (inclusive).
		/// </summary>
		public int To { get; }

		public ExitCodeRange(int from, int to)
		{
			From = from;
			To = to;
		}

		public ExitCodeRange(int exitCode) : this(exitCode, exitCode)
		{
		}

		public bool Contains(int exitCode) => exitCode >= From && exitCode <= To;

		public bool Equals(ExitCodeRange other) => this.From == other.From && this.To == other.To;

		public override bool Equals(object obj) => (obj is ExitCodeRange other) ? Equals(other) : false;

		public override int GetHashCode()
		{
			if (From == To)
				return From.GetHashCode();
			else
				return From.GetHashCode() ^ To.GetHashCode();
		}

		public override string ToString() => (From == To) ? From.ToString() : $"{From}-{To}";


		public static IEnumerable<ExitCodeRange> ParseList(string list)
		{
			if (list.Trim().Length == 0)
				return Enumerable.Empty<ExitCodeRange>();
			else
				return list.Split(',').Select(p => p.Trim()).Select(ParseItem);

			ExitCodeRange ParseItem(string part)
			{
				var rangeParts = part.Split('-');
				if (part.Length == 0 || rangeParts.Length > 2)
					throw new CommandLineArgumentException($"Invalid exit code range: \"{list}\"");

				if (rangeParts.Length == 1)
				{
					return new ExitCodeRange(ParseExitCode(part));
				}
				else
				{
					var from = ParseExitCode(rangeParts[0]);
					var to = ParseExitCode(rangeParts[1]);
					if (from <= to)
						return new ExitCodeRange(from, to);
					else
						return new ExitCodeRange(to, from);
				}
			}

			int ParseExitCode(string item)
			{
				if (Int32.TryParse(item, out int value))
					return value;
				else
					throw new CommandLineArgumentException($"Invalid exit code: \"{item}\"");
			}
		}
	}
}