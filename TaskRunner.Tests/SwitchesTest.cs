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
		public void SendModeSwitch_Invalid_ThrowsCommandLineArgumentException()
		{
			var args = new[] { "-s", "foo", "--", "cmd.exe" };
			var switches = new Switches(args);

			Should.Throw<CommandLineArgumentException>(() => switches.Parse())
					.Message.ShouldContain("Invalid send mode");
		}

		[TestMethod]
		public void SendModeSwitch_ValidValueInCorrectCase_ReturnsValue()
		{
			var args = new[] { "-s", "Always", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SendMode.ShouldBe(SendMode.Always);
		}

		[TestMethod]
		public void SendModeSwitch_ValidValueInIncorrectCase_ReturnsValue()
		{
			var args = new[] { "-s", "onFailurE", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SendMode.ShouldBe(SendMode.OnFailure);
		}

		[TestMethod]
		public void SendModeSwitch_NotSpecified_DefaultIsOnFailureOrOutput()
		{
			var args = new[] { "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SendMode.ShouldBe(SendMode.OnFailureOrOutput);
		}


		[TestMethod]
		public void ExitCodeSwitch_NotSpecified_DefaultsToZero()
		{
			var args = new[] { "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SuccessExitCodes.ShouldBe(new[] { new ExitCodeRange(0) });
		}

		[TestMethod]
		public void ExitCodeSwitch_Specified_ReturnsParsedListOfValues()
		{
			var args = new[] { "--exitcode", "1,3", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SuccessExitCodes.ShouldBe(new[] { new ExitCodeRange(1), new ExitCodeRange(3) });
		}

		[TestMethod]
		public void ExitCodeSwitch_SpecifiedTwice_ReturnsConcatenatedParsedListOfValues()
		{
			var args = new[] { "--exitcode", "1-3", "-e", "7", "--", "cmd.exe" };
			var switches = new Switches(args);

			switches.Parse();

			switches.SuccessExitCodes.ShouldBe(new[] { new ExitCodeRange(1, 3), new ExitCodeRange(7) }, ignoreOrder: true);
		}


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
		public void ConfigureSwitch_NoTargetCommand_NoExceptionThrown()
		{
			var args = new[] { "--configure", "-h", "mail.example.com" };
			var switches = new Switches(args);

			Should.NotThrow(() => switches.Parse());
		}

		[TestMethod]
		public void ConfigureSwitch_NoTargetCommand_NoTargetCommandLine()
		{
			var args = new[] { "--configure", "-h", "mail.example.com" };
			var switches = new Switches(args);

			switches.Parse();

			switches.TargetCommandLine.ShouldBeEmpty();
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
