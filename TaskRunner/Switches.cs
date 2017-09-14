/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */

using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace TaskRunner
{
	/// <summary>
	/// Command line argument switches.
	/// </summary>
	class Switches
	{
		private readonly string[] m_args;
		private int m_targetStart = -1;


		/// <summary>
		/// Gets any e-mail settings provided on the command line.
		/// </summary>
		public EmailSettings EmailSettings { get; private set; } = new EmailSettings();

		public IEnumerable<string> TargetCommandLine => m_args.Skip(m_targetStart);
		
		/// <summary>
		/// Should the supplied e-mail settings be written to the registry for future use?
		/// </summary>
		public bool ConfigureEmail { get; private set; }


		public Switches(string[] args)
		{
			m_args = args;
		}

		/// <summary>
		/// Parse the switches.
		/// </summary>
		/// <exception cref="CommandLineArgumentException">thrown if there is an error parsing the switches</exception>
		public void Parse()
		{
			string host = null;
			MailAddress from = null;
			MailAddress to = null;

			for (int i = 0; i < m_args.Length; i++)
			{
				string arg = m_args[i];

				if (!arg.StartsWith("-"))
				{
					throw new CommandLineArgumentException($"Unrecognised switch: \"{arg}\"");
				}
				else if (arg == "--")
				{
					m_targetStart = i + 1;
					break;
				}

				switch (arg)
				{
					case "-h":
					case "--host":
						host = ShiftNextArg();
						break;
					case "-f":
					case "--from":
						var fromString = ShiftNextArg();
						if (!Email.TryParseAddress(fromString, out from))
							throw new CommandLineArgumentException($"Invalid 'from' e-mail address: \"{fromString}\"");
						break;
					case "-t":
					case "--to":
						var toString = ShiftNextArg();
						if (!Email.TryParseAddress(toString, out to))
							throw new CommandLineArgumentException($"Invalid 'to' e-mail address: \"{toString}\"");
						break;
					case "-c":
					case "--configure":
						ConfigureEmail = true;
						break;
					default:
						throw new CommandLineArgumentException($"Unrecognised switch: \"{arg}\"");
				}

				string ShiftNextArg()
				{
					if (i + 1 >= m_args.Length)
						throw new CommandLineArgumentException($"Missing argument to {arg} switch");
					return m_args[++i];
				}
			}

			EmailSettings = new EmailSettings(host: host, from: from, to: to);

			if (!ConfigureEmail && (m_targetStart >= m_args.Length || m_targetStart <= 0))
				throw new CommandLineArgumentException("No program to run was specified");
		}
	}
}
