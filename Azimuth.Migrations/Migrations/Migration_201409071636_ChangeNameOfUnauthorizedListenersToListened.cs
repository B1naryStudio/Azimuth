using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409071636)]
    public class Migration_201409071636_ChangeNameOfUnauthorizedListenersToListened:Migration
    {
        public override void Up()
        {
            Rename.Table("UnauthorizedListeners").To("Listened");
        }

        public override void Down()
        {
            Rename.Table("Listened").To("UnauthorizedListeners");
        }
    }
}
