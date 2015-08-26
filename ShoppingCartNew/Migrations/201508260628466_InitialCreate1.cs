namespace ShoppingCartNew.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CartItems");
            AlterColumn("dbo.CartItems", "CartItemId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Products", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AddPrimaryKey("dbo.CartItems", "CartItemId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CartItems");
            AlterColumn("dbo.Products", "UnitPrice", c => c.Double());
            AlterColumn("dbo.CartItems", "CartItemId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.CartItems", "CartItemId");
        }
    }
}
