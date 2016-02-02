using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Meteor
{
	public class MongoDocument
	{
		public string _id;

		public MongoDocument() {}

		public virtual Dictionary<string,object> toDictionary () {
			return new Dictionary<string,object> {
				{"_id",_id}
			};
		}
	}
}

