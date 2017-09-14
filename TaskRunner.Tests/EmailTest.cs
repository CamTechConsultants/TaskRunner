/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	/// <summary>
	/// Unit tests for the Email class.
	/// </summary>
	[TestClass]
	public class EmailTest
	{
		[TestMethod]
		public void TryParseAddress_JustEmailAddress_ReturnsInstanceWithoutDisplayName()
		{
			bool result = Email.TryParseAddress("joe.bloggs@example.com", out MailAddress address);

			result.ShouldBe(true);
			address.Host.ShouldBe("example.com");
			address.User.ShouldBe("joe.bloggs");
			address.DisplayName.ShouldBe("");
		}

		[TestMethod]
		public void TryParseAddress_EmailAddressAndName_ReturnsInstanceWithEmailAndDisplayName()
		{
			bool result = Email.TryParseAddress("Joe Bloggs <joe.bloggs@example.com>", out MailAddress address);

			result.ShouldBe(true);
			address.Host.ShouldBe("example.com");
			address.User.ShouldBe("joe.bloggs");
			address.DisplayName.ShouldBe("Joe Bloggs");
		}

		[TestMethod]
		public void TryParseAddress_JustName_ReturnsFalse()
		{
			bool result = Email.TryParseAddress("Joe Bloggs", out MailAddress address);

			result.ShouldBe(false);
			address.ShouldBeNull();
		}
	}
}
