using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.MembersAndGroups.Messages
{
	public class CreateGroupMessage
	{
		public string Id;

		public CreateGroupMessage(string id) => Id = id;
	}
}
