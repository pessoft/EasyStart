namespace EasyStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCommentCauseCancelInOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "CommentCauseCancel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "CommentCauseCancel");
        }
    }
}
