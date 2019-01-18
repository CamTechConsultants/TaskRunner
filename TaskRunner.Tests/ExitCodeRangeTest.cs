/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	[TestClass]
	public class ExitCodeRangeTest
	{
		[TestMethod]
		public void Contains_SingleValueRangeAndExitCodeIsEqual_ReturnsTrue()
		{
			var range = new ExitCodeRange(42);
			var result = range.Contains(42);
			result.ShouldBeTrue();
		}

		[TestMethod]
		public void Contains_SingleValueRangeAndExitCodeIsLower_ReturnsFalse()
		{
			var range = new ExitCodeRange(42);
			var result = range.Contains(41);
			result.ShouldBeFalse();
		}

		[TestMethod]
		public void Contains_SingleValueRangeAndExitCodeIsHigher_ReturnsFalse()
		{
			var range = new ExitCodeRange(0);
			var result = range.Contains(41);
			result.ShouldBeFalse();
		}

		[TestMethod]
		public void Contains_ExitCodeIsWithinRange_ReturnsTrue()
		{
			var range = new ExitCodeRange(1, 5);
			var result = range.Contains(3);
			result.ShouldBeTrue();
		}

		[TestMethod]
		public void Contains_ExitCodeIsBelowRange_ReturnsFalse()
		{
			var range = new ExitCodeRange(1, 5);
			var result = range.Contains(0);
			result.ShouldBeFalse();
		}

		[TestMethod]
		public void Contains_ExitCodeIsAboveRange_ReturnsFalse()
		{
			var range = new ExitCodeRange(1, 5);
			var result = range.Contains(42);
			result.ShouldBeFalse();
		}


		[TestMethod]
		public void ParseList_SingleValue_SingleValueReturned()
		{
			var result = ExitCodeRange.ParseList("1");
			result.ShouldBe(new[] { new ExitCodeRange(1) });
		}

		[TestMethod]
		public void ParseList_TwoValues_TwoValuesReturned()
		{
			var result = ExitCodeRange.ParseList("1,2");
			result.ShouldBe(new[] { new ExitCodeRange(1), new ExitCodeRange(2) }, ignoreOrder: true);
		}

		[TestMethod]
		public void ParseList_Range_RangeReturned()
		{
			var result = ExitCodeRange.ParseList("0-3");
			result.ShouldBe(new[] { new ExitCodeRange(0, 3) });
		}

		[TestMethod]
		public void ParseList_RangeInWrongOrder_RangeOrderIsSwitched()
		{
			var result = ExitCodeRange.ParseList("3-1");
			result.ShouldBe(new[] { new ExitCodeRange(1, 3) });
		}

		[TestMethod]
		public void ParseList_MixtureOfValuesAndRanges_AllValuesReturned()
		{
			var result = ExitCodeRange.ParseList("1,3-5,10-9,7");
			var expected = new[] { new ExitCodeRange(1), new ExitCodeRange(3, 5), new ExitCodeRange(9, 10), new ExitCodeRange(7) };
			result.ShouldBe(expected, ignoreOrder: true);
		}

		[TestMethod]
		public void ParseList_EmptyString_ReturnsEmptyList()
		{
			var result = ExitCodeRange.ParseList("");
			result.ShouldBe(new ExitCodeRange[0]);
		}

		[TestMethod]
		public void ParseList_EmptyItem_ThrowsCommandLineArgumentException()
		{
			Should.Throw<CommandLineArgumentException>(() => ExitCodeRange.ParseList("1,").ToList());
		}

		[TestMethod]
		public void ParseList_HangingRangeRight_ThrowsCommandLineArgumentException()
		{
			Should.Throw<CommandLineArgumentException>(() => ExitCodeRange.ParseList("1-").ToList());
		}

		[TestMethod]
		public void ParseList_HangingRangeLeft_ThrowsCommandLineArgumentException()
		{
			Should.Throw<CommandLineArgumentException>(() => ExitCodeRange.ParseList("-1").ToList());
		}

		[TestMethod]
		public void ParseList_ThreeWayRange_ThrowsCommandLineArgumentException()
		{
			Should.Throw<CommandLineArgumentException>(() => ExitCodeRange.ParseList("1-2-3").ToList());
		}
	}
}
