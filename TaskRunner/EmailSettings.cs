/*
 * John Hall <john.hall@camtechconsultants.com>
 * Copyright (c) Cambridge Technology Consultants Ltd. All rights reserved.
 */

using System.Net.Mail;

namespace TaskRunner
{
	class EmailSettings
	{
		public string Host { get; }

		public MailAddress From { get; }

		public MailAddress To { get; }

		public bool IsValid
		{
			get
			{
				return Host != null && From != null && To != null;
			}
		}

		public EmailSettings(string host = null, MailAddress from = null, MailAddress to = null)
		{
			Host = host;
			From = from;
			To = to;
		}

		public EmailSettings MergeIn(EmailSettings other)
		{
			return new EmailSettings(
					host: other.Host ?? this.Host,
					from: other.From ?? this.From,
					to: other.To ?? this.To);
		}
	}
}
