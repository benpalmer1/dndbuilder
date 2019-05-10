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


namespace DndBuilder.WebApi.Test
{
    [Obsolete("Needed for mono version of NUnit",false)]
    [TestFixture()]
    public class DatabaseTests
    {
        private void Setup()
        {

        }

        [TearDown]
        private void TearDown()
        {

        }

        [Test()]
        public void DatabaseSetupExists()
        {
            Setup();
        }
        [Test()]
        public void DatabaseSetupNotExists()
        {

        }

        [Test()]
        public void GetCharacterNameNotExists()
        {
            Setup();
        }
        [Test()]
        public void GetCharacterNameExists()
        {
            Setup();
        }

        [Test()]
        public void AddCharacterNameNotExists()
        {
            Setup();
        }
        [Test()]
        public void AddCharacterNameExists()
        {
            Setup();
        }
    }
}