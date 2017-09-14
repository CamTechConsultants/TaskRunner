/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskRunner.Tests
{
	/// <summary>
	/// Unit tests for the Switches class.
	/// </summary>
	[TestClass]
	public class SwitchesTest
	{
		[TestMethod]
		public void NoMailSettings_EmailSettingsNotNull()
		{
			var args = new[] { "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.ShouldNotBeNull();
		}

		[TestMethod]
		public void HostSwitch_HostProvided_ReturnsEmailSettingsWithHostSet()
		{
			var args = new[] { "-h", "mail.example.com", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.Host.ShouldBe("mail.example.com");
		}

		[TestMethod]
		public void FromSwitch_BareAddressProvided_ReturnsEmailSettingsWithFromSet()
		{
			var args = new[] { "-f", "joe.bloggs@example.com", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.From.Address.ShouldBe("joe.bloggs@example.com");
		}

		[TestMethod]
		public void FromSwitch_NameAndAddressProvided_ReturnsEmailSettingsWithFromSet()
		{
			var args = new[] { "--from", "Joe <joe.bloggs@example.com>", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.From.DisplayName.ShouldBe("Joe");
			switches.EmailSettings.From.Address.ShouldBe("joe.bloggs@example.com");
		}

		[TestMethod]
		public void ToSwitch_BareAddressProvided_ReturnsEmailSettingsWithToSet()
		{
			var args = new[] { "-t", "joe.bloggs@example.com", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.To.Address.ShouldBe("joe.bloggs@example.com");
		}

		[TestMethod]
		public void ToSwitch_NameAndAddressProvided_ReturnsEmailSettingsWithToSet()
		{
			var args = new[] { "--to", "Joe <joe.bloggs@example.com>", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.EmailSettings.To.DisplayName.ShouldBe("Joe");
			switches.EmailSettings.To.Address.ShouldBe("joe.bloggs@example.com");
		}


		[TestMethod]
		public void UnrecognisedSwitch_ThrowsCommandLineArgsException()
		{
			var args = new[] { "-K", "x", "--", "cmd.exe" };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.ShouldContain("Unrecognised switch");
		}

		[TestMethod]
		public void MissingArgumentToSwitch_ThrowsCommandLineArgsException()
		{
			var args = new[] { "-h", "--", "cmd.exe" };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.Contains("Unrecognised switch");
		}

		[TestMethod]
		public void MissingArgumentToSwitchAndNoTargetCommandLine_ThrowsCommandLineArgsException()
		{
			var args = new[] { "-h" };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.Contains("Missing argument");
		}

		[TestMethod]
		public void NoTargetCommand_ThrowsCommandLineArgsException()
		{
			var args = new[] { "--", };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.Contains("No program");
		}

		[TestMethod]
		public void NoDoubleDash_ThrowsCommandLineArgsException()
		{
			var args = new[] { "-h", "mail.example.com", "cmd.exe" };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.Contains("No program");
		}

		[TestMethod]
		public void TargetCommandLine_AllArgumentsReturned()
		{
			var args = new[] { "-h", "mail.example.com", "--", "myprogram.exe", "-f", "foo" };
			var switches = new Switches(args);

			switches.Parse();

			switches.TargetCommandLine.ShouldBe(new[] { "myprogram.exe", "-f", "foo" });
		}
	}
}
