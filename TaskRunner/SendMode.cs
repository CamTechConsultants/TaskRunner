/*
 * John Hall <john.hall@xjtag.com>
 * Copyright (c) Midas Yellow Ltd. All rights reserved.
 */

namespace TaskRunner
{
	/// <summary>
	/// Defines the different modes for sending e-mails.
	/// </summary>
	enum SendMode
	{
		/// <summary>
		/// Only send an e-mail if the task return a non-zero exit code.
		/// </summary>
		OnFailure,

		/// <summary>
		/// Send an e-mail if the task return a non-zero exit code or if it produced any output (the default).
		/// </summary>
		OnFailureOrOutput,

		/// <summary>
		/// Always send an e-mail.
		/// </summary>
		Always,
	}
}