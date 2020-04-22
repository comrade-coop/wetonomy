using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.MembersAndGroups.Messages
{
	public class CreateMemberMessage
	{
		public string Id;

		public CreateMemberMessage(string id) => Id = id;
	}
}
