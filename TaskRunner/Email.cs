/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System;
using System.Net.Mail;

namespace TaskRunner
{
	static class Email
	{
		public static bool TryParseAddress(string address, out MailAddress mailAddress)
		{
			try
			{
				mailAddress = new MailAddress(address);
				return true;
			}
			catch (FormatException)
			{
				mailAddress = null;
				return false;
			}
		}
	}
}