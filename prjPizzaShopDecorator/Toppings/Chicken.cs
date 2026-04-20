namespace prjPizzaShopDecorator
{
    class Chicken : MenuItemDecorator
    {
        public Chicken(MenuItem Decorated) : base(Decorated)
        {
            this.Description = "Chicken";
            this.Price = 35;    
        }
    }
}
           
