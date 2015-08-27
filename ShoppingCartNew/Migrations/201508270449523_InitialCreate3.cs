namespace ShoppingCartNew.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShoppingCartViewModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CartTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.CartItems", "ShoppingCartViewModel_ID", c => c.Int());
            CreateIndex("dbo.CartItems", "ShoppingCartViewModel_ID");
            AddForeignKey("dbo.CartItems", "ShoppingCartViewModel_ID", "dbo.ShoppingCartViewModels", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "ShoppingCartViewModel_ID", "dbo.ShoppingCartViewModels");
            DropIndex("dbo.CartItems", new[] { "ShoppingCartViewModel_ID" });
            DropColumn("dbo.CartItems", "ShoppingCartViewModel_ID");
            DropTable("dbo.ShoppingCartViewModels");
        }
    }
}
