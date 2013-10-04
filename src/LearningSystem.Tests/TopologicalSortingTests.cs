using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LearningSystem.Models;
using LearningSystem.App.Controllers;
using LearningSystem.App.ViewModels;

namespace LearningSystem.Tests
{
    [TestClass]
    public class TopologicalSortingTests
    {
        private List<Lesson> CreateLessons(int count)
        {
            List<Lesson> lessons = new List<Lesson>();
            for (int i = 0; i < count; i++)
            {
                lessons.Add(new Lesson { LessonId = i });
            }

            return lessons;
        }

        [TestMethod]
        public void StraightChainSorting()
        {
            List<Lesson> lessons = CreateLessons(4);

            lessons[1].Requirements.Add(lessons[0]);
            lessons[2].Requirements.Add(lessons[1]);
            lessons[3].Requirements.Add(lessons[2]);

            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(0, sortedLessons[0].Id);
            Assert.AreEqual(3, sortedLessons[3].Id);
            Assert.AreEqual(0, sortedLessons[0].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[1].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[2].LevelInSkillTree);
            Assert.AreEqual(3, sortedLessons[3].LevelInSkillTree);
        }

        [TestMethod]
        public void ReverseChainSorting()
        {
            List<Lesson> lessons = CreateLessons(4);

            lessons[0].Requirements.Add(lessons[1]);
            lessons[1].Requirements.Add(lessons[2]);
            lessons[2].Requirements.Add(lessons[3]);

            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(3, sortedLessons[0].Id);
            Assert.AreEqual(0, sortedLessons[3].Id);
            Assert.AreEqual(0, sortedLessons[0].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[1].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[2].LevelInSkillTree);
            Assert.AreEqual(3, sortedLessons[3].LevelInSkillTree);
        }

        [TestMethod]
        public void DiamondChain()
        {
            List<Lesson> lessons = CreateLessons(4);

            lessons[1].Requirements.Add(lessons[0]);
            lessons[2].Requirements.Add(lessons[0]);
            lessons[3].Requirements.Add(lessons[2]);

            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(0, sortedLessons[0].Id);
            Assert.AreEqual(3, sortedLessons[3].Id);
            Assert.AreEqual(0, sortedLessons[0].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[1].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[2].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[3].LevelInSkillTree);
        }

        [TestMethod]
        public void FourLevelPyramid()
        {
            List<Lesson> lessons = CreateLessons(10);

            lessons[1].Requirements.Add(lessons[0]);
            lessons[2].Requirements.Add(lessons[0]);

            lessons[3].Requirements.Add(lessons[1]);
            lessons[4].Requirements.Add(lessons[2]);
            lessons[5].Requirements.Add(lessons[1]);

            lessons[6].Requirements.Add(lessons[3]);
            lessons[7].Requirements.Add(lessons[4]);
            lessons[8].Requirements.Add(lessons[5]);
            lessons[9].Requirements.Add(lessons[3]);

            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(0, sortedLessons[0].Id);
            Assert.AreEqual(9, sortedLessons[9].Id);

            Assert.AreEqual(1, sortedLessons[1].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[2].LevelInSkillTree);

            Assert.AreEqual(2, sortedLessons[3].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[4].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[5].LevelInSkillTree);

            Assert.AreEqual(3, sortedLessons[6].LevelInSkillTree);
            Assert.AreEqual(3, sortedLessons[7].LevelInSkillTree);
            Assert.AreEqual(3, sortedLessons[8].LevelInSkillTree);
            Assert.AreEqual(3, sortedLessons[9].LevelInSkillTree);
        }

        [TestMethod]
        public void FourLevelReversePyramid()
        {
            List<Lesson> lessons = CreateLessons(10);

            lessons[0].Requirements.Add(lessons[1]);
            lessons[0].Requirements.Add(lessons[2]);

            lessons[1].Requirements.Add(lessons[3]);
            lessons[2].Requirements.Add(lessons[4]);
            lessons[1].Requirements.Add(lessons[5]);

            lessons[3].Requirements.Add(lessons[6]);
            lessons[4].Requirements.Add(lessons[7]);
            lessons[5].Requirements.Add(lessons[8]);
            lessons[3].Requirements.Add(lessons[9]);  

            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(3, sortedLessons[9].LevelInSkillTree);

            Assert.AreEqual(2, sortedLessons[8].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[7].LevelInSkillTree);

            Assert.AreEqual(1, sortedLessons[6].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[5].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[4].LevelInSkillTree);

            Assert.AreEqual(0, sortedLessons[3].LevelInSkillTree);
            Assert.AreEqual(0, sortedLessons[2].LevelInSkillTree);
            Assert.AreEqual(0, sortedLessons[1].LevelInSkillTree);
            Assert.AreEqual(0, sortedLessons[0].LevelInSkillTree);
        }

        [TestMethod]
        public void TwoColumnsSorting()
        {
            List<Lesson> lessons = CreateLessons(6);

            lessons[4].Requirements.Add(lessons[2]);
            lessons[5].Requirements.Add(lessons[3]);

            lessons[2].Requirements.Add(lessons[0]);
            lessons[3].Requirements.Add(lessons[1]);


            SkillController controller = new SkillController(null);

            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            controller.TopologicalSort(sortedLessons, lessons);

            Assert.AreEqual(0, sortedLessons[0].LevelInSkillTree);
            Assert.AreEqual(0, sortedLessons[1].LevelInSkillTree);

            Assert.AreEqual(1, sortedLessons[2].LevelInSkillTree);
            Assert.AreEqual(1, sortedLessons[3].LevelInSkillTree);

            Assert.AreEqual(2, sortedLessons[4].LevelInSkillTree);
            Assert.AreEqual(2, sortedLessons[5].LevelInSkillTree);
        }
    }
}
