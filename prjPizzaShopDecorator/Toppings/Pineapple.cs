namespace prjPizzaShopDecorator
{
    class Pineapple : MenuItemDecorator
    {
        public Pineapple(MenuItem Decorated) : base(Decorated)
        {
            this.Description = "Pineapple";
            this.Price = 40;    
        }
    }
}
           
