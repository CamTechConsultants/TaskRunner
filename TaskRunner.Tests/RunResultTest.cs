/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	/// <summary>
	/// Unit tests for the RunResult class.
	/// </summary>
	[TestClass]
	public class RunResultTest
	{
		[TestMethod]
		public void DefaultInstance_OutputPropertyReturnsEmptyString()
		{
			var result = default(RunResult);
			result.Output.ShouldBeEmpty();
		}

		[TestMethod]
		public void Ctor_SetOutputToNull_OutputPropertyReturnsEmptyString()
		{
			var result = new RunResult(1, null);
			result.Output.ShouldBeEmpty();
		}
	}
}
