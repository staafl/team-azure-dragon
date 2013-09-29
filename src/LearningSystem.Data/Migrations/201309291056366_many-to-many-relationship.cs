namespace LearningSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manytomanyrelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lessons", "Lesson_LessonId", "dbo.Lessons");
            DropForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Lessons", new[] { "Lesson_LessonId" });
            DropIndex("dbo.AspNetUsers", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.LessonLessons",
                c => new
                    {
                        Lesson_LessonId = c.Int(nullable: false),
                        Lesson_LessonId1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Lesson_LessonId, t.Lesson_LessonId1 })
                .ForeignKey("dbo.Lessons", t => t.Lesson_LessonId)
                .ForeignKey("dbo.Lessons", t => t.Lesson_LessonId1)
                .Index(t => t.Lesson_LessonId)
                .Index(t => t.Lesson_LessonId1);
            
            CreateTable(
                "dbo.ApplicationUserApplicationUsers",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id1 = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.ApplicationUser_Id1 })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.SkillApplicationUsers",
                c => new
                    {
                        Skill_SkillId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Skill_SkillId, t.ApplicationUser_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_SkillId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Skill_SkillId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.LessonApplicationUsers",
                c => new
                    {
                        Lesson_LessonId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Lesson_LessonId, t.ApplicationUser_Id })
                .ForeignKey("dbo.Lessons", t => t.Lesson_LessonId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Lesson_LessonId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ExerciseApplicationUsers",
                c => new
                    {
                        Exercise_ExerciseId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Exercise_ExerciseId, t.ApplicationUser_Id })
                .ForeignKey("dbo.Exercises", t => t.Exercise_ExerciseId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Exercise_ExerciseId)
                .Index(t => t.ApplicationUser_Id);
            
            DropColumn("dbo.AspNetUsers", "ApplicationUser_Id");
            DropColumn("dbo.Lessons", "Lesson_LessonId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lessons", "Lesson_LessonId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ExerciseApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExerciseApplicationUsers", "Exercise_ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.LessonApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LessonApplicationUsers", "Lesson_LessonId", "dbo.Lessons");
            DropForeignKey("dbo.SkillApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SkillApplicationUsers", "Skill_SkillId", "dbo.Skills");
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LessonLessons", "Lesson_LessonId1", "dbo.Lessons");
            DropForeignKey("dbo.LessonLessons", "Lesson_LessonId", "dbo.Lessons");
            DropIndex("dbo.ExerciseApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ExerciseApplicationUsers", new[] { "Exercise_ExerciseId" });
            DropIndex("dbo.LessonApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.LessonApplicationUsers", new[] { "Lesson_LessonId" });
            DropIndex("dbo.SkillApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.SkillApplicationUsers", new[] { "Skill_SkillId" });
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.ApplicationUserApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.LessonLessons", new[] { "Lesson_LessonId1" });
            DropIndex("dbo.LessonLessons", new[] { "Lesson_LessonId" });
            DropTable("dbo.ExerciseApplicationUsers");
            DropTable("dbo.LessonApplicationUsers");
            DropTable("dbo.SkillApplicationUsers");
            DropTable("dbo.ApplicationUserApplicationUsers");
            DropTable("dbo.LessonLessons");
            CreateIndex("dbo.AspNetUsers", "ApplicationUser_Id");
            CreateIndex("dbo.Lessons", "Lesson_LessonId");
            AddForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Lessons", "Lesson_LessonId", "dbo.Lessons", "LessonId");
        }
    }
}
