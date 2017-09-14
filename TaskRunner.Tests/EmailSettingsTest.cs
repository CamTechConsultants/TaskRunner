/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	[TestClass]
	public class EmailSettingsTest
	{
		private static readonly MailAddress ExampleAddress1 = new MailAddress("joe.bloggs@example.com");
		private static readonly MailAddress ExampleAddress2 = new MailAddress("fred.bloggs@example.com");


		[TestMethod]
		public void IsValid_AllFieldsPresent_ReturnsTrue()
		{
			var settings = new EmailSettings(host: "mail.example.com", from: ExampleAddress1, to: ExampleAddress2);
			bool isValid = settings.IsValid;
			isValid.ShouldBe(true);
		}

		[TestMethod]
		public void IsValid_NoHost_ReturnsFalse()
		{
			var settings = new EmailSettings(from: ExampleAddress1, to: ExampleAddress2);
			bool isValid = settings.IsValid;
			isValid.ShouldBe(false);
		}

		[TestMethod]
		public void IsValid_NoFromAddress_ReturnsFalse()
		{
			var settings = new EmailSettings(host: "mail.example.com", to: ExampleAddress2);
			bool isValid = settings.IsValid;
			isValid.ShouldBe(false);
		}

		[TestMethod]
		public void IsValid_NoToAddress_ReturnsFalse()
		{
			var settings = new EmailSettings(host: "mail.example.com", from: ExampleAddress1);
			bool isValid = settings.IsValid;
			isValid.ShouldBe(false);
		}


		[TestMethod]
		public void MergeIn_ThisFieldsAllNull_AllFieldsReplaced()
		{
			var settings1 = new EmailSettings();
			var settings2 = new EmailSettings(host: "mail.example.com", from: ExampleAddress1, to: ExampleAddress2);

			var result = settings1.MergeIn(settings2);

			result.Host.ShouldBe("mail.example.com");
			result.From.ShouldBe(ExampleAddress1);
			result.To.ShouldBe(ExampleAddress2);
		}

		[TestMethod]
		public void MergeIn_ThisFieldsAllSetAndOtherFieldsAllNull_AllFieldsLeftUnchanged()
		{
			var settings1 = new EmailSettings(host: "mail.example.com", from: ExampleAddress1, to: ExampleAddress2);
			var settings2 = new EmailSettings();

			var result = settings1.MergeIn(settings2);

			result.Host.ShouldBe("mail.example.com");
			result.From.ShouldBe(ExampleAddress1);
			result.To.ShouldBe(ExampleAddress2);
		}

		[TestMethod]
		public void MergeIn_ThisFieldsAllSetAndOtherFieldsAllSet_ThisFieldsAllReplaced()
		{
			var settings1 = new EmailSettings(host: "mail.example.com", from: ExampleAddress1, to: ExampleAddress1);
			var settings2 = new EmailSettings(host: "mail.example.com", from: ExampleAddress2, to: ExampleAddress2);

			var result = settings1.MergeIn(settings2);

			result.Host.ShouldBe("mail.example.com");
			result.From.ShouldBe(ExampleAddress2);
			result.To.ShouldBe(ExampleAddress2);
		}
	}
}