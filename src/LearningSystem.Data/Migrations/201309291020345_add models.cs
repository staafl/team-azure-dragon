namespace LearningSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exercises",
                c => new
                    {
                        ExerciseId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(storeType: "ntext"),
                        Order = c.Int(nullable: false),
                        LessonId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ExerciseId)
                .ForeignKey("dbo.Lessons", t => t.LessonId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.LessonId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Lessons",
                c => new
                    {
                        LessonId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Description = c.String(storeType: "ntext"),
                        SkillId = c.Int(nullable: false),
                        Lesson_LessonId = c.Int(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LessonId)
                .ForeignKey("dbo.Lessons", t => t.Lesson_LessonId)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.Lesson_LessonId)
                .Index(t => t.SkillId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        SkillId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(storeType: "ntext"),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SkillId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        Statement = c.String(nullable: false, maxLength: 60),
                        Order = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        AnswerType = c.Int(nullable: false),
                        AnswerContent = c.String(nullable: false, maxLength: 1000),
                        ExerciseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.Exercises", t => t.ExerciseId, cascadeDelete: true)
                .Index(t => t.ExerciseId);
            
            AddColumn("dbo.AspNetUsers", "Email", c => c.String());
            AddColumn("dbo.AspNetUsers", "Facebook", c => c.String());
            AddColumn("dbo.AspNetUsers", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "ApplicationUser_Id");
            AddForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Skills", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Lessons", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Exercises", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Questions", "ExerciseId", "dbo.Exercises");
            DropForeignKey("dbo.Lessons", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.Lessons", "Lesson_LessonId", "dbo.Lessons");
            DropForeignKey("dbo.Exercises", "LessonId", "dbo.Lessons");
            DropIndex("dbo.Skills", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Lessons", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Exercises", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Questions", new[] { "ExerciseId" });
            DropIndex("dbo.Lessons", new[] { "SkillId" });
            DropIndex("dbo.Lessons", new[] { "Lesson_LessonId" });
            DropIndex("dbo.Exercises", new[] { "LessonId" });
            DropColumn("dbo.AspNetUsers", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUsers", "Facebook");
            DropColumn("dbo.AspNetUsers", "Email");
            DropTable("dbo.Questions");
            DropTable("dbo.Skills");
            DropTable("dbo.Lessons");
            DropTable("dbo.Exercises");
        }
    }
}
