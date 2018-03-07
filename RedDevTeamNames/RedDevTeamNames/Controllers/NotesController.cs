using RedDevTeamNames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Configuration;
using MongoDB.Bson;

namespace RedDevTeamNames.Controllers
{
    public class NotesController : ApiController
    {
        string collectionName = "Notes";  // production
        //string collectionName = "NotesTest";  // testing

        MongoDatabase mongoDatabase;

        bool testing = false;
        List<Note> noteList = new List<Note>();

        public NotesController()
        {
            testing = false;
        }

        public NotesController(List<Note> FakeDataList)
        {
            collectionName = "testNotes";
            noteList = FakeDataList;
            testing = true;
        }


        private MongoDatabase RetreiveMongohqDb()
        {
            string connectionString = "mongodb://reddevteam:bcuser17@ds062448.mlab.com:62448/reddevteam";
            //TEST MONGOURL
            MongoUrl mongoURL = new MongoUrl(connectionString);
            //MongoUrl myMongoURL = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoHQ"].ConnectionString);
            MongoClient mongoClient = new MongoClient(mongoURL);
            MongoServer server = mongoClient.GetServer();
            return mongoClient.GetServer().GetDatabase("reddevteam");
        }

        public IEnumerable<Note> GetAllNotes()
        {
            if (!testing)
            {
                mongoDatabase = RetreiveMongohqDb();
                try
                {
                    var mongoList = mongoDatabase.GetCollection(collectionName).FindAll().AsEnumerable();
                    noteList = (from note in mongoList
                                select new Note
                                {
                                    Id = note["_id"].AsString,
                                    Subject = note["Subject"].AsString,
                                    Details = note["Details"].AsString,
                                    Priority = note["Priority"].AsInt32
                                }).ToList();
                }

                catch (Exception ex)
                {
                    throw new ApplicationException("failed to get data from Mongo");
                }
            }

            noteList.Sort();
            return noteList;
        }

        public IHttpActionResult GetNote(string id)
        {
            if (!testing)
            {
                mongoDatabase = RetreiveMongohqDb();

                try
                {
                    var mongoList = mongoDatabase.GetCollection(collectionName).FindAll().AsEnumerable();
                    noteList = (from nextNote in mongoList
                                select new Note
                                {
                                    Id = nextNote["_id"].AsString,
                                    Subject = nextNote["Subject"].AsString,
                                    Details = nextNote["Details"].AsString,
                                    Priority = nextNote["Priority"].AsInt32,
                                }).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            var note = noteList.FirstOrDefault((p) => p.Subject == id);
            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        //public IEnumerable<Note> GetAllNotes()
        //{
        //    mongoDatabase = RetreiveMongohqDb();
        //    List<Note> noteList = new List<Note>();
        //    try {
        //        var mongoList = mongoDatabase.GetCollection("NotesTest").FindAll().AsEnumerable();
        //        noteList = (from note in mongoList
        //                    select new Note                    
        //            {
        //                Id = note["_id"].AsString,                        
        //                Subject = note["Subject"].AsString,                        
        //                Details = note["Details"].AsString,                        
        //                Priority = note["Priority"].AsInt32                    
        //            }).ToList();
        //    }
        //    catch (Exception ex) {
        //        throw new ApplicationException("failed to get data from Mongo");
        //    }
        //    noteList.Sort();
        //    return noteList;

        //    //return notes;
        //}

        //public IHttpActionResult GetNote(string id)
        //{
        //    mongoDatabase = RetreiveMongohqDb();

        //    List<Note> noteList = new List<Note>();
        //    try
        //    {
        //        var mongoList = mongoDatabase.GetCollection("Notes").FindAll().AsEnumerable();
        //        noteList = (from nextNote in mongoList
        //                    select new Note
        //                    {
        //                        Id = nextNote["_id"].AsString,
        //                        Subject = nextNote["Subject"].AsString,
        //                        Details = nextNote["Details"].AsString,
        //                        Priority = nextNote["Priority"].AsInt32,
        //                    }).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //    var note = noteList.FirstOrDefault((p) => p.Subject == id);
        //    if (note == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(note);
        //}

        [HttpDelete]
        public HttpResponseMessage Delete(string id)
        {
            bool found = true;
            string subject = id;
            if (!testing)
            {
                try
                {
                    mongoDatabase = RetreiveMongohqDb();
                    var mongoCollection = mongoDatabase.GetCollection(collectionName);
                    var query = Query.EQ("Subject", subject);
                    WriteConcernResult results = mongoCollection.Remove(query);

                    if (results.DocumentsAffected < 1)
                    {
                        found = false;
                    }
                }
                catch (Exception ex)
                {
                    found = false;
                }
                HttpResponseMessage response = new HttpResponseMessage();
                if (!found)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            } else
            {
                int noteCount = noteList.Count;
                try
                {
                    noteList.RemoveAll(a => a.Subject == id);
                    if(noteCount < noteList.Count )
                    {
                        found = false;
                    }
                }
                catch
                {
                    found = false;
                }
                HttpResponseMessage response = new HttpResponseMessage();
                if (!found)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }


            }
        }

        [HttpPost]
        public Note Save(Note newNote)
        {
            if (!testing)
            {
                mongoDatabase = RetreiveMongohqDb();
                var noteList = mongoDatabase.GetCollection(collectionName);
                WriteConcernResult result;
                bool hasError = false;
                if (string.IsNullOrEmpty(newNote.Id))
                {
                    newNote.Id = ObjectId.GenerateNewId().ToString();
                    result = noteList.Insert<Note>(newNote);
                    hasError = result.HasLastErrorMessage;
                }
                else
                {
                    IMongoQuery query = Query.EQ("_id", newNote.Id);
                    IMongoUpdate update = Update
                        .Set("Subject", newNote.Subject)
                        .Set("Details", newNote.Details)
                        .Set("Priority", newNote.Priority);
                    result = noteList.Update(query, update);
                    hasError = result.HasLastErrorMessage;
                }
                if (!hasError)
                {
                    return newNote;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                try
                {
                    noteList.Add(newNote);
                }
                catch
                {
                    Note emptyNote = new Note();
                    return emptyNote;
                }
                return newNote;
            }
        }

    }
}

