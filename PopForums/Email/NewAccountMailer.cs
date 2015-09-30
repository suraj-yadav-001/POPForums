﻿// TODO: New account mailer

//using System;
//using System.Net.Mail;
//using PopForums.Configuration;
//using PopForums.Models;

//namespace PopForums.Email
//{
	//public interface INewAccountMailer
	//{
	//	SmtpStatusCode Send(User user, string verifyUrl);

	//	/// <summary>
	//	/// Used to deliver the text for a welcome e-mail, where the user is already 
	//	/// approved. The default implementation uses Resources.RegisterEmailThankYou. 
	//	/// It uses the following string format items:
	//	/// {0} Forum title (from settings)
	//	/// {1} Mail signature (from settings)
	//	/// {2} Environment.NewLine
	//	/// </summary>
	//	string NewUserApprovedEmail { get; }

	//	/// <summary>
	//	/// Used to deliver the text for a welcome e-mail, where the user is must follow  
	//	/// a verification link. The default implementation uses Resources.RegisterEmailThankYou. 
	//	/// It uses the following string format items:
	//	/// {0} Forum title (from settings)
	//	/// {1} Verification URL + auth code (generated by calling code)
	//	/// {2} Verification URL
	//	/// {3} Authorization key (from user object)
	//	/// {4} Mail signature (from settings)
	//	/// {5} Environment.NewLine
	//	/// </summary>
	//	string NewUserVerifyEmail { get; }
	//}

//	public class NewAccountMailer : INewAccountMailer
//	{
//		public NewAccountMailer(ISettingsManager settingsManager, ISmtpWrapper smtpWrapper)
//		{
//			_settingsManager = settingsManager;
//			_smtpWrapper = smtpWrapper;
//		}

//		private readonly ISettingsManager _settingsManager;
//		private readonly ISmtpWrapper _smtpWrapper;

//		public SmtpStatusCode Send(User user, string verifyUrl)
//		{
//			var settings = _settingsManager.Current;
//			if (String.IsNullOrWhiteSpace(settings.MailerAddress))
//				throw new Exception("There is no MailerAddress to send e-mail from. Perhaps you didn't set up the settings.");
//			var message = new MailMessage(
//				new MailAddress(settings.MailerAddress, settings.ForumTitle), 
//				new MailAddress(user.Email, user.Name));
//			message.Subject = String.Format(Resources.RegisterEmailSubject, settings.ForumTitle);
//			string body;
//			if (settings.IsNewUserApproved)
//				body = String.Format(NewUserApprovedEmail, settings.ForumTitle, settings.MailSignature, "\r\n");
//			else
//				body = String.Format(NewUserVerifyEmail, settings.ForumTitle, verifyUrl + "/" + user.AuthorizationKey, verifyUrl, user.AuthorizationKey, settings.MailSignature, "\r\n");
//			message.Body = body;
//			return _smtpWrapper.Send(message);
//		}

//		public virtual string NewUserApprovedEmail
//		{
//			get { return Resources.RegisterEmailThankYou; }
//		}

//		public virtual string NewUserVerifyEmail
//		{
//			get { return Resources.RegisterEmailThankYouVerify; }
//		}
//	}
//}
