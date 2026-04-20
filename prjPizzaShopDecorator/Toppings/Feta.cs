namespace prjPizzaShopDecorator
{
    class Feta : MenuItemDecorator
    {
        public Feta(MenuItem Decorated) : base(Decorated)
        {
            this.Description = "Feta";
            this.Price = 15;    
        }
    }
}
           
