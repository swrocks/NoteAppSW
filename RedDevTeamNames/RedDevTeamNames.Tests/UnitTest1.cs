using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RedDevTeamNames.Models;
using RedDevTeamNames.Controllers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;

namespace RedDevTeamNames.Tests
{
    [TestClass]
    public class TestNotesController
    {
        List<Note> noteList = new List<Note>();
        private List<Note> GenerateFakeDataList()
        {
            List<Note> workingList = new List<Note>();
            for (int i = 0; i < 3; i++)
            {
                Note nextNote = new Note();
                nextNote.Id = i.ToString();
                nextNote.Subject = "Test" + i.ToString();
                nextNote.Details = "Test" + i.ToString() + " Details";
                nextNote.Priority = i;
                workingList.Add(nextNote);
            }
            return workingList;
        }
        //=======================================================================
        // test first API   GetAllNotes()
        [TestMethod]
        // first test local logic, using fake data
        public void GetAllFakeNotes_ShouldReturnAllNotes()
        {
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(testNotes); // use 1 of 2 constructors

            var result = controller.GetAllNotes() as List<Note>;
            Assert.AreEqual(testNotes.Count, result.Count);
        }
        [TestMethod]
        // now test against test data in mongo
        public void GetAllMongoNotes_ShouldReturnAllNotes()
        {
            // need to modify Controller to point to NotesTest
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(); // use the other constructor

            var result = controller.GetAllNotes() as List<Note>;
            Assert.AreEqual(testNotes.Count, result.Count);
        }
        //=======================================================================
        // test 2nd API   GetNote(string id)
        [TestMethod]
        // first test local logic, using fake data
        public void GetFakeNote_ShouldReturnParticularNote()
        {
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(testNotes); // use 1 of 2 constructors

            IHttpActionResult result = controller.GetNote("Test2");
            var contentResult = result as OkNegotiatedContentResult<Note>;

            Assert.AreEqual(testNotes[2].Subject, contentResult.Content.Subject);

        }
        //=======================================================================
        [TestMethod]
        //test mongo delete return ok
        public void DeleteRetunsOk_Mongo()
        {
            Note noteToBeReSaved = new Note();
            string testNote = "Test2";
            noteToBeReSaved.Details = testNote + " Details";
            noteToBeReSaved.Priority = 2;
            noteToBeReSaved.Subject = testNote;
            var controller = new NotesController();


            var response = controller.Delete(testNote);
            //add deleted test note back to the DB to prevent other tests from failing.
            controller.Save(noteToBeReSaved);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }
        //=======================================================================
        [TestMethod]
        //test fake delete return ok
        public void DeleteRetunsOk_Fake()
        {
            List<Note> testNotes = GenerateFakeDataList();
            string testNote = "Test2";
            var controller = new NotesController(testNotes);


            var response = controller.Delete(testNote);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }
        //=======================================================================
        [TestMethod]
        // now test against test data in mongo
        public void GetMongoNote_ShouldReturnParticularNote()
        {
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(); // use other constructors

            IHttpActionResult result = controller.GetNote("Test2");
            var contentResult = result as OkNegotiatedContentResult<Note>; ;

            Assert.AreEqual(testNotes[2].Subject, contentResult.Content.Subject);

        }

        [TestMethod]
        public void GetFakeNotes_ShouldReturnNotFoundIfBroken()
        {
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(testNotes); // use 1 of 2 constructors

            IHttpActionResult result = controller.GetNote("Test5");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetMongoNotes_ShouldReturnNotFoundIfBroken()
        {
            List<Note> testNotes = GenerateFakeDataList();
            var controller = new NotesController(); // use 2 of 2 constructor

            IHttpActionResult result = controller.GetNote("Test5");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        //=======================================================================
        [TestMethod]
        public void FakeData_AddNewNote_Success()
        {
            List<Note> testNotes = GenerateFakeDataList();
            string subjectToDelete = "Test Subject";
            Note noteToAdd = new Note();
            noteToAdd.Details = "Test Details";
            noteToAdd.Priority = 2;
            noteToAdd.Subject = subjectToDelete;
            var controller = new NotesController(testNotes);

            Note result = controller.Save(noteToAdd);

            Assert.AreEqual(noteToAdd, result);
        }
        //=======================================================================
        [TestMethod]
        public void MongoData_AddNewNote_Success()
        {
            string subjectToDelete = "Test Subject";
            Note noteToAdd = new Note();
            noteToAdd.Details = "Test Details";
            noteToAdd.Priority = 2;
            noteToAdd.Subject = subjectToDelete;
            var controller = new NotesController();

            Note result = controller.Save(noteToAdd);
            controller.Delete(subjectToDelete);

            Assert.AreEqual(noteToAdd, result);
        }
    }
}
