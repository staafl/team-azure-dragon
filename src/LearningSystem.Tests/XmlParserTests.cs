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

        [TestMethod]
        public void PlacesObjectInDb()
        {
            var mockDbProvider = new Mock<IUoWLearningSystem>();

            FileStream _stream = new FileStream(string.Format("..//..//TestingZips//picking-mushrooms.zip"), FileMode.Open);

            var file = new Mock<HttpPostedFileBase>();
            
            file.Setup(x => x.InputStream).Returns(_stream);
            file.Setup(x => x.ContentLength).Returns((int)_stream.Length);
            file.Setup(x => x.FileName).Returns(_stream.Name);


            mockDbProvider.Setup(x => x.Skills.Add(It.IsAny<Skill>()));

            UploadController controller = new UploadController(mockDbProvider.Object);

            controller.SaveSkill(file.Object);

            mockDbProvider.Verify(x => x.Skills.Add(It.IsAny<Skill>()));
        }
    }
}
