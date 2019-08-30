﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PopForums.Models;
using PopForums.Mvc.Areas.Forums.Services;
using PopForums.Services;

namespace PopForums.Mvc.Areas.Forums.Controllers
{
	[Area("Forums")]
	public class PrivateMessagesController : Controller
	{
		public PrivateMessagesController(IPrivateMessageService privateMessageService, IUserService userService, IUserRetrievalShim userRetrievalShim)
		{
			_privateMessageService = privateMessageService;
			_userService = userService;
			_userRetrievalShim = userRetrievalShim;
		}

		private readonly IPrivateMessageService _privateMessageService;
		private readonly IUserService _userService;
		private readonly IUserRetrievalShim _userRetrievalShim;

		public static string Name = "PrivateMessages";

		public ActionResult Index(int page = 1)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			PagerContext pagerContext;
			var privateMessages = _privateMessageService.GetPrivateMessages(user, PrivateMessageBoxType.Inbox, page, out pagerContext);
			ViewBag.PagerContext = pagerContext;
			return View(privateMessages);
		}

		public ActionResult Archive(int page = 1)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			PagerContext pagerContext;
			var privateMessages = _privateMessageService.GetPrivateMessages(user, PrivateMessageBoxType.Archive, page, out pagerContext);
			ViewBag.PagerContext = pagerContext;
			return View(privateMessages);
		}

		public async Task<ActionResult> View(int id)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			var pm = await _privateMessageService.Get(id);
			if (!_privateMessageService.IsUserInPM(user, pm))
				return StatusCode(403);
			var posts = await _privateMessageService.GetPosts(pm);
			var model = new PrivateMessageView
			{
				PrivateMessage = pm,
				Posts = posts
			};
			_privateMessageService.MarkPMRead(user, pm);
			return View(model);
		}

		public ActionResult Create(int? id)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			ViewBag.UserIDs = " ";
			if (id.HasValue)
			{
				var targetUser = _userService.GetUser(id.Value);
				ViewBag.UserIDs = targetUser.UserID.ToString(CultureInfo.InvariantCulture);
				ViewBag.UserID = targetUser.UserID.ToString(CultureInfo.InvariantCulture);
				ViewBag.TargetUserID = targetUser.UserID;
				ViewBag.TargetUserName = targetUser.Name;
			}
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> CreateOne(string subject, string fullText, int userID)
		{
			return await Create(subject, fullText, userID.ToString(CultureInfo.InvariantCulture));
		}

		[HttpPost]
		public async Task<ActionResult> Create(string subject, string fullText, string userIDs)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			if (string.IsNullOrWhiteSpace(userIDs) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(fullText))
			{
				ViewBag.Warning = Resources.PMCreateWarnings;
				return View("Create");
			}
			var ids = userIDs.Split(new[] { ',' }).Select(i => Convert.ToInt32(i));
			var users = ids.Select(id => _userService.GetUser(id)).ToList();
			await _privateMessageService.Create(subject, fullText, user, users);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Reply(int id, string fullText)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			var pm = await _privateMessageService.Get(id);
			if (!_privateMessageService.IsUserInPM(user, pm))
				return StatusCode(403);
			if (String.IsNullOrEmpty(fullText))
				fullText = String.Empty;
			await _privateMessageService.Reply(pm, fullText, user);
			return RedirectToAction("View", new { id });
		}

		public JsonResult GetNames(string id)
		{
			var users = _userService.SearchByName(id);
			var projection = users.Select(u => new { u.UserID, value = u.Name });
			return Json(projection);
		}

		[HttpPost]
		public async Task<ActionResult> ArchivePM(int id)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			var pm = await _privateMessageService.Get(id);
			if (!_privateMessageService.IsUserInPM(user, pm))
				return StatusCode(403);
			_privateMessageService.Archive(user, pm);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> UnarchivePM(int id)
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return StatusCode(403);
			var pm = await _privateMessageService.Get(id);
			if (!_privateMessageService.IsUserInPM(user, pm))
				return StatusCode(403);
			_privateMessageService.Unarchive(user, pm);
			return RedirectToAction("Archive");
		}

		public ContentResult NewPMCount()
		{
			var user = _userRetrievalShim.GetUser(HttpContext);
			if (user == null)
				return Content(String.Empty);
			var count = _privateMessageService.GetUnreadCount(user);
			return Content(count.ToString());
		}
	}
}
