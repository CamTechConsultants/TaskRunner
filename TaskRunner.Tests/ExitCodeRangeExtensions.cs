/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	[TestClass]
	public class ExitCodeRangeExtensions
	{
		[TestMethod]
		public void Contains_EmptyList_ReturnsFalse()
		{
			var list = new ExitCodeRange[0];
			var result = list.Contains(42);
			result.ShouldBeFalse();
		}

		[TestMethod]
		public void Contains_IsContainedWithinFirstItemInList_ReturnsTrue()
		{
			var list = new[] { new ExitCodeRange(1, 3), new ExitCodeRange(7), new ExitCodeRange(9, 10) };
			var result = list.Contains(2);
			result.ShouldBeTrue();
		}

		[TestMethod]
		public void Contains_IsContainedWithinSubsequentItemInList_ReturnsTrue()
		{
			var list = new[] { new ExitCodeRange(1, 3), new ExitCodeRange(7), new ExitCodeRange(9, 10) };
			var result = list.Contains(7);
			result.ShouldBeTrue();
		}

		[TestMethod]
		public void Contains_IsNotContainedWithinAnyItemInList_ReturnsFalse()
		{
			var list = new[] { new ExitCodeRange(1, 3), new ExitCodeRange(7), new ExitCodeRange(9, 10) };
			var result = list.Contains(6);
			result.ShouldBeFalse();
		}
	}
}
