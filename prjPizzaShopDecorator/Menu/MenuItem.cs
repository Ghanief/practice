using System;
namespace prjPizzaShopDecorator
{
    // this class define what everything on the program should have
    // This abstract class defines the interface for objects.
    // that can have responsibilites added to them dynamically

    abstract class MenuItem
    {
        // Each pizza (thin/thick base) or topping has to have a description
        // and a price
        public abstract string GetDescription();

        public abstract decimal GetPrice();

    }
}