using System;

namespace prjPizzaShopDecorator
{
// This is the actual object we will wrapping.
// A thin base pizza
    class Thin: MenuItem
    {
        public override string GetDescription()
        {
            return "Thin base";
        }

        public override decimal GetPrice()
        {
            return 20;
        }
    }
}