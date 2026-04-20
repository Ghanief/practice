namespace prjPizzaShopDecorator
{
    class Cheese : MenuItemDecorator
    {
        public Cheese(MenuItem Decorated) : base(Decorated)
        {
            this.Description = "Cheese";
            this.Price = 10;
                
        }
    }
}
           
