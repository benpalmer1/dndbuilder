/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Basic unit testing class for the SQLite database component of the Dndbuilder assignment.
 * Uses NUnit from mono, as per the no-3rd-party-libraries requirement.
 */

using System;
using NUnit.Framework;

using DndBuilder.WebApi.DndBuilderDatabase;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using DndBuilder.WebApi.Models;
using static DndBuilder.WebApi.DndBuilderDatabase.Database;

namespace DndBuilder.WebApi.Test
{
    [Obsolete("For mono version of NUnit",false)]
    [TestFixture]
    public class DatabaseTests
    {
        public Database Setup()
        {
            Database db = new Database();
            db.DatabaseSetup();

            // insert a few characters here:
            db.AddCharacter(new DndCharacter()
            {
                Name = "Erufu",
                Age = 40,
                Level = 5,
                Biography = "From Teldrassil",
                Gender = "Male",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", HitDie = 10, Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            });

            db.AddCharacter(new DndCharacter()
            {
                Name = "BoomShift",
                Age = 40,
                Level = 10,
                Biography = "From ThunderBluff",
                Gender = "Male",
                Race = new DndRace() { Name = "Tauren", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", HitDie = 10, Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            });

            db.AddCharacter(new DndCharacter()
            {
                Name = "Brutal",
                Age = 40,
                Level = 14,
                Biography = "From Eastern Kingdoms",
                Gender = "Female",
                Race = new DndRace() { Name = "Blood Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Death Knight", HitDie = 10, Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            });

            return db;
        }

        // Deletes test database once complete.
        [TearDown]
        public void TearDown()
        {
            File.Delete(Database.DATABASE_FILENAME);
        }

        [Test]
        public void DatabaseSetupExists()
        {
            Database db1 = Setup();

            Database db2 = new Database();
            db2.DatabaseSetup();

            // No changes, database still exists
            Assert.That(File.Exists(Database.DATABASE_FILENAME));

            Assert.That(db2.GetCharacter(1).Equals(db1.GetCharacter(1)));
            Assert.That(db2.GetCharacter(2).Equals(db1.GetCharacter(2)));
            Assert.That(db2.GetCharacter(3).Equals(db1.GetCharacter(3)));
        }

        [Test]
        public void DatabaseSetupNotExists()
        {
            Assert.That(!File.Exists(Database.DATABASE_FILENAME));

            Setup();

            Assert.That(File.Exists(Database.DATABASE_FILENAME));
        }

        [Test]
        public void GetCharacterNameNotExists()
        {
            Database db = Setup();

            try
            {
                db.GetCharacter("John");

                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message, Is.EqualTo("Unable to get character. Character does not exist in the database.")); 
            }
        }

        [Test]
        public void GetCharacterNameExists()
        {
            Database db = Setup();

            DndCharacter erufu = new DndCharacter()
            {
                Name = "Erufu",
                Age = 40,
                Level = 5,
                Biography = "From Teldrassil",
                Gender = "Male",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", HitDie = 10, Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            DndCharacter erufuOriginal = db.GetCharacter("Erufu");

            Assert.That(db.IsCharacterNameInUse("Erufu").Equals(true));
        }

        [Test]
        public void AddCharacterNameNotExists()
        {
            Database db = Setup();

            DndCharacter newchar = new DndCharacter()
            {
                Name = "Homer Simpson",
                Age = 35,
                Level = 1,
                Biography = "From Springfield",
                Gender = "Male",
                Race = new DndRace() { Name = "Human", RacialBonuses = new int[] { 0, 0, 0, 0, 0, 0 } },
                CharacterClass = new DndClass() { Name = "Lazy", HitDie = 0, Spellcaster = false },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            Assert.That(db.IsCharacterNameInUse("Homer Simpson").Equals(false));

            db.AddCharacter(newchar);

            Assert.That(db.IsCharacterNameInUse("Homer Simpson").Equals(true));
        }

        [Test]
        public void AddCharacterNameExists()
        {
            Database db = Setup();
            int originalCount = db.GetAllCharacters().Count;

            DndCharacter erufu = new DndCharacter()
            {
                Name = "Erufu",
                Age = 40,
                Level = 5,
                Biography = "From Teldrassil",
                Gender = "Male",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", HitDie = 10, Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            try
            {
                db.AddCharacter(erufu);

                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message, Is.EqualTo("Unable to add character. Character by that name already exists."));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }
        }

        [Test]
        public void AddCharacterInvalid()
        {
            Database db = Setup();
            int originalCount = db.GetAllCharacters().Count;

            // Each missing a few things...
            DndCharacter invalid = new DndCharacter()
            {
                Name = "Get Invalidated1",
                Age = 40,
                Level = 80,
                Gender = "Yes",
                Biography = "From Teldrassil",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            DndCharacter invalid2 = new DndCharacter()
            {
                Age = 40,
                Level = 80,
                Gender = "Yes",
                CharacterClass = new DndClass() { Name = "Druid", Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            DndCharacter invalid3 = new DndCharacter()
            {
                Name = "Get Invalidated2",
                Age = 40,
                Level = 4,
                Gender = "Yes",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", Spellcaster = true },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            DndCharacter invalid4 = new DndCharacter()
            {
                Name = "Get Invalidated3",
                Age = 4456,
                Gender = "Yes",
                Race = new DndRace() { Name = "Night Elf", RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                AbilityScores = new int[] { 2, 2, 2, 10, 2, 2 }
            };

            DndCharacter invalid5 = new DndCharacter()
            {
                Name = "Get Invalidated4",
                Age = 40,
                Level = 44,
                Race = new DndRace() { RacialBonuses = new int[] { 0, 0, 0, 0, 10, 0 } },
                CharacterClass = new DndClass() { Name = "Druid", Spellcaster = true }
            };

            try
            {
                db.AddCharacter(invalid);
                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message.StartsWith("Unable to add character. Character fails server validation.", StringComparison.Ordinal));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }

            try
            {
                db.AddCharacter(invalid2);
                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message.StartsWith("Unable to add character. Character fails server validation.", StringComparison.Ordinal));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }

            try
            {
                db.AddCharacter(invalid3);
                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message.StartsWith("Unable to add character. Character fails server validation.", StringComparison.Ordinal));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }

            try
            {
                db.AddCharacter(invalid4);
                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message.StartsWith("Unable to add character. Character fails server validation.", StringComparison.Ordinal));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }

            try
            {
                db.AddCharacter(invalid5);
                Assert.Fail();
            }
            catch (DatabaseException e)
            {
                Assert.That(e.Message.StartsWith("Unable to add character. Character fails server validation.", StringComparison.Ordinal));
                Assert.That(db.GetAllCharacters().Count, Is.EqualTo(originalCount));
            }
        }
    }
}