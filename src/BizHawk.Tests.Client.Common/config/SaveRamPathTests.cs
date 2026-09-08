using System.IO;

using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Tests.Client.Common.config
{
	/// <summary>
	/// <para>Save RAM is named after the ROM file, not the database's title for the game.</para>
	/// <para>
	/// Those titles are canonical per game, so byte-identical copies of one ROM -- which is how
	/// several instances of a game are given separate saves -- all resolved to the same save file and
	/// overwrote each other. A ROM the database does not recognise already fell back to its filename;
	/// these pin that as the rule rather than an accident of coverage.
	/// </para>
	/// </summary>
	[TestClass]
	public class SaveRamPathTests
	{
		private static string SaveRamFileNameFor(string title, string? romFileName)
		{
			var game = new GameInfo
			{
				Name = title,
				FileName = romFileName,
				System = VSystemID.Raw.GBA,
			};
			return Path.GetFileName(new PathEntryCollection().SaveRamAbsolutePath(game, movie: null));
		}

		[TestMethod]
		public void NamedAfterTheRomFile()
		{
			Assert.AreEqual(
				"Pokemon Ruby1.SaveRAM",
				SaveRamFileNameFor("Pokemon - Ruby Version (USA, Europe) (Rev 2)", "Pokemon Ruby1"));
		}

		[TestMethod]
		public void CopiesOfOneRomGetSeparateSaves()
		{
			// The whole point: same database entry, same title, different files.
			const string Title = "Pokemon - Ruby Version (USA, Europe) (Rev 2)";
			Assert.AreNotEqual(SaveRamFileNameFor(Title, "Pokemon Ruby1"), SaveRamFileNameFor(Title, "Pokemon Ruby2"));
		}

		[TestMethod]
		public void FallsBackToTheTitleWithoutAFile()
		{
			// Archive members and cores that load without a file have no filename to use.
			Assert.AreEqual("Pokemon - Ruby Version.SaveRAM", SaveRamFileNameFor("Pokemon - Ruby Version", romFileName: null));
			Assert.AreEqual("Pokemon - Ruby Version.SaveRAM", SaveRamFileNameFor("Pokemon - Ruby Version", romFileName: ""));
		}

		[TestMethod]
		public void TitleFallbackIsStillMadeFilesystemSafe()
		{
			// FilesystemSafeName's job, and it still has to happen on the fallback path.
			Assert.AreEqual("Wario Land 3 - Fushigi na Orgel.SaveRAM", SaveRamFileNameFor("Wario Land 3: Fushigi na Orgel", romFileName: null));
		}
	}
}
