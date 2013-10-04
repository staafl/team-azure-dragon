using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web;
using Moq;
using LearningSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using LearningSystem.Models;
using LearningSystem.App.AppLogic;
using System.Reflection;
using LearningSystem.App.ViewModels;
using System.Web.Mvc;
using TeamAzureDragon.Utils;
using LearningSystem.App.Areas.Administration.Controllers;

namespace LearningSystem.Tests
{
    [TestClass]
    public class XmlParserTests
    {
        [TestInitialize]
        public void SetUp()
        {

        }

        private byte[] LoadBytesFromFile(string path)
        {
            FileStream _stream = new FileStream(string.Format(path), FileMode.Open);
            byte[] bytes = new byte[_stream.Length];
            _stream.Read(bytes, 0, bytes.Length);
            _stream.Close();

            return bytes;
        }

        private static Mock<HttpPostedFileBase> MockAddMethod(MemoryStream memory)
        {
            var file = new Mock<HttpPostedFileBase>();

            file.Setup(x => x.InputStream).Returns(memory);
            file.Setup(x => x.ContentLength).Returns((int)memory.Length);
            return file;
        }

        [TestMethod]
        public void PlacesObjectInDb()
        {
            var mockDbProvider = new Mock<IUoWLearningSystem>();

            byte[] stream = LoadBytesFromFile("..//..//TestingZips//picking-mushrooms.zip");

            MemoryStream memory = new MemoryStream(stream);


            var file = MockAddMethod(memory);
            //file.Setup(x => x.FileName).Returns(_stream.Name);

            mockDbProvider.Setup(x => x.Skills.Add(It.IsAny<Skill>()));

            UploadController controller = new UploadController(mockDbProvider.Object);

            controller.SaveSkill(file.Object);

            mockDbProvider.Verify(x => x.Skills.Add(It.IsAny<Skill>()));
        }

       

        [TestMethod]
        public void CorrectSkillProperties()
        {
            var mockDbProvider = new Mock<IUoWLearningSystem>();

            byte[] stream = LoadBytesFromFile("..//..//TestingZips//picking-mushrooms.zip");
            MemoryStream memory = new MemoryStream(stream);

            var file = MockAddMethod(memory);

            mockDbProvider.Setup(x => x.Skills.Add(It.IsAny<Skill>()));

            UploadController controller = new UploadController(mockDbProvider.Object);

            controller.SaveSkill(file.Object);

            mockDbProvider.Verify(x => x.Skills.Add(It.Is<Skill>(
                s =>
                s.Name == "Mushroom Picking" &&
                s.Description == "Learn how to pick mushrooms like a pro")));
        }

        [TestMethod]
        public void CorrectLessonProperties()
        {
            var mockDbProvider = new Mock<IUoWLearningSystem>();

            byte[] stream = LoadBytesFromFile("..//..//TestingZips//picking-mushrooms.zip");
            MemoryStream memory = new MemoryStream(stream);

            var file = MockAddMethod(memory);

            mockDbProvider.Setup(x => x.Skills.Add(It.IsAny<Skill>()));

            UploadController controller = new UploadController(mockDbProvider.Object);

            controller.SaveSkill(file.Object);

            mockDbProvider.Verify(x => x.Skills.Add(It.Is<Skill>(
                s =>
                s.Lessons.Count == 6 &&
                s.Lessons.ElementAt(5).Requirements.First().LessonId == 4 &&
                s.Lessons.ElementAt(5).Exercises.Single().ExerciseId == 7 &&
                s.Lessons.ElementAt(5).LessonId == 6
                )));

            mockDbProvider.Verify(x => x.Skills.Add(It.Is<Skill>(
                s =>
                s.Lessons.ElementAt(1).LessonId == 2 &&
                s.Lessons.ElementAt(1).Requirements.First().LessonId == 1 &&
                s.Lessons.ElementAt(1).Exercises.ElementAt(0).ExerciseId == 2 &&
                s.Lessons.ElementAt(1).Exercises.ElementAt(1).ExerciseId == 3 
                )));
            
        }
    }
}
