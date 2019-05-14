/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 10MAY19
 * 
 * Purpose:
 * Basic unit testing class for the SQLite database component of the Dndbuilder assignment.
 * Uses NUnit from mono, as per the no-3rd-party-libraries requirement.
 */

using System;
using NUnit.Framework;

using DndBuilder.WebApi.DndBuilderDatabase;


namespace DndBuilder.WebApi.Test
{
    [Obsolete("For mono version of NUnit",false)]
    [TestFixture()]
    public class DatabaseTests
    {
        private Database db = new Database();

        [SetUp]
        public void Setup()
        {
            db.DatabaseSetup();

            // insert a few characters here:
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test()]
        public void DatabaseSetupExists()
        {
            Database db2 = new Database();
            db2.DatabaseSetup();

            // check only one exists?
        }
        [Test()]
        public void DatabaseSetupNotExists()
        {

        }

        [Test()]
        public void GetCharacterNameNotExists()
        {

        }
        [Test()]
        public void GetCharacterNameExists()
        {

        }

        [Test()]
        public void AddCharacterNameNotExists()
        {

        }
        [Test()]
        public void AddCharacterNameExists()
        {

        }
    }
}