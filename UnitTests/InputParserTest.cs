using System.Collections.Generic;
using DuplicateFileReporter.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
	[TestClass]
	public class InputParserTest
	{
		[TestMethod]
		public void InputParserConstructorTest()
		{
			var inputStrings = new List<string>
			                   	{
			                   		"--double-hash",
									"--path",
									@"C:\Users\Andrew\Documents\Programming Projects\Ruby\GitHub\Media File Manipulation Framework\MVC Revamp",
			                   	};

			var parser = new InputParser(inputStrings);

			Assert.IsFalse(parser.FoundInvalidArgs);
		}
	}
}
