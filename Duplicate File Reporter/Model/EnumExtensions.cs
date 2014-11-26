using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DuplicateFileReporter.Model
{
	public static class EnumExtensionMethods
	{
		public static IList<Attribute> GetAttributes(this Enum e)
		{
			return e.GetAttributes(null);
		}

		public static IList<Attribute> GetAttributes(this Enum e, Type requestedAttributeType)
		{
			var type = e.GetType();
			var mi = type.GetMember(e.ToString());
			if ((mi != null) && (mi.Length > 0))
			{
				object[] attrs = requestedAttributeType == null ? mi[0].GetCustomAttributes(false) : mi[0].GetCustomAttributes(requestedAttributeType, false);
				if (attrs != null)
				{
					return attrs.OfType<Attribute>().ToList<Attribute>();
				}
			}

			return null;
		}

		public static string GetDescription(this Enum e)
		{
			var attrs = e.GetAttributes(typeof(DescriptionAttribute));
			if ((attrs != null) && (attrs.Count > 0))
			{
				return ((DescriptionAttribute)attrs[0]).Description;
			}

			return String.Empty;
		}
	}
}
