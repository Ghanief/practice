using System;

namespace prjPizzaShopDecorator
{
// This is the actual object we will wrapping.
// A thick base pizza
    class Thick: MenuItem
    {
        public override string GetDescription()
        {
            return "Thick base";
        }

        public override decimal GetPrice()
        {
            return 50;
        }
    }
}