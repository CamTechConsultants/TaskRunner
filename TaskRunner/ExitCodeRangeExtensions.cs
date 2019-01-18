/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */

using System.Collections.Generic;
using System.Linq;

namespace TaskRunner
{
	static class ExitCodeRangeExtensions
	{
		public static bool Contains(this IEnumerable<ExitCodeRange> exitCodeList, int exitCode)
		{
			return exitCodeList.Any(r => r.Contains(exitCode));
		}
	}
}
