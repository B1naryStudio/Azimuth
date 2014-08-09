using System;
using System.Linq;
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201408091845)]
    public class Migration_201408091845_Initial : Migration
    {
        public override void Up()
        {
            var scripts = Resources.CreateSqlServerContent.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            scripts.ForEach(Execute.Sql);
        }

        public override void Down()
        {
            var scripts = Resources.DropSqlServerContent.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            scripts.ForEach(Execute.Sql);
        }
    }
}