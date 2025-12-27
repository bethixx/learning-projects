using System;
using System.Collections.Generic;
using System.Linq;

public interface IOrderState
{
    void AddProduct(Order order, string product);
    void SubmitOrder(Order order);
    void ConfirmPayment(Order order);
    void PackProduct(Order order, string product);
    void ShipOrder(Order order);
    void CancelOrder(Order order);
    void ShowOrderDetails(Order order);
}

public class Order
{
    public Dictionary<string, bool> Products { get; } = new();
    private IOrderState currentState;

    public Order()
    {
        currentState = new CreatedState();
    }

    public void SetState(IOrderState state)
    {
        currentState = state;
    }

    public void AddProduct(string product)
    {
        currentState.AddProduct(this, product);
    }

    public void SubmitOrder()
    {
        currentState.SubmitOrder(this);
    }

    public void ConfirmPayment()
    {
        currentState.ConfirmPayment(this);
    }

    public void PackProduct(string product)
    {
        currentState.PackProduct(this, product);
    }

    public void ShipOrder()
    {
        currentState.ShipOrder(this);
    }

    public void CancelOrder()
    {
        currentState.CancelOrder(this);
    }

    public void ShowOrderDetails()
    {
        currentState.ShowOrderDetails(this);
    }

    public void ClearOrder()
    {
        Products.Clear();
    }
}

public class CreatedState : IOrderState
{
    public void AddProduct(Order order, string product)
    {
        if (order.Products.ContainsKey(product))
        {
            Console.WriteLine("Produkt już dodany do zamówienia.");
        }
        else
        {
            order.Products.Add(product, false);
            Console.WriteLine($"Dodano produkt: {product}");
        }
    }

    public void SubmitOrder(Order order)
    {
        if (order.Products.Count == 0)
        {
            Console.WriteLine("Nie można złożyć zamówienia bez produktów.");
        }
        else
        {
            Console.WriteLine("Zamówienie zostało złożone.");
            order.SetState(new SubmittedState());
        }
    }

    public void ConfirmPayment(Order order)
    {
        Console.WriteLine("Nie można potwierdzić płatności w stanie Created.");
    }

    public void PackProduct(Order order, string product)
    {
        Console.WriteLine("Nie można spakować produktu w stanie Created.");
    }

    public void ShipOrder(Order order)
    {
        Console.WriteLine("Nie można wysłać zamówienia w stanie Created.");
    }

    public void CancelOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało anulowane.");
        order.ClearOrder();
    }

    public void ShowOrderDetails(Order order)
    {
        Console.WriteLine("Zamówienie w trakcie tworzenia:");
        foreach (var product in order.Products)
        {
            Console.WriteLine($"- {product.Key}");
        }
    }
}

public class SubmittedState : IOrderState
{
    public void AddProduct(Order order, string product)
    {
        Console.WriteLine("Nie można dodać produktów do złożonego zamówienia.");
    }

    public void SubmitOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało już złożone.");
    }

    public void ConfirmPayment(Order order)
    {
        Console.WriteLine("Płatność została potwierdzona.");
        order.SetState(new PaidState());
    }

    public void PackProduct(Order order, string product)
    {
        Console.WriteLine("Nie można spakować produktu przed opłaceniem zamówienia.");
    }

    public void ShipOrder(Order order)
    {
        Console.WriteLine("Nie można wysłać zamówienia przed opłaceniem.");
    }

    public void CancelOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało anulowane.");
        order.ClearOrder();
    }

    public void ShowOrderDetails(Order order)
    {
        Console.WriteLine("Zamówienie oczekuje na opłatę.");
    }
}

public class PaidState : IOrderState
{
    public void AddProduct(Order order, string product)
    {
        Console.WriteLine("Nie można dodać produktów do opłaconego zamówienia.");
    }

    public void SubmitOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało już złożone.");
    }

    public void ConfirmPayment(Order order)
    {
        Console.WriteLine("Płatność została już potwierdzona.");
    }

    public void PackProduct(Order order, string product)
    {
        if (!order.Products.ContainsKey(product))
        {
            Console.WriteLine("Produkt nie znajduje się w zamówieniu.");
        }
        else if (order.Products[product])
        {
            Console.WriteLine("Produkt już spakowany.");
        }
        else
        {
            order.Products[product] = true;
            Console.WriteLine($"Produkt {product} został spakowany.");
        }
    }

    public void ShipOrder(Order order)
    {
        if (order.Products.All(p => p.Value))
        {
            Console.WriteLine("Wysyłanie zamówienia.");
            order.SetState(new ShippedState());
        }
        else
        {
            Console.WriteLine("Nie wszystkie produkty zostały spakowane.");
        }
    }

    public void CancelOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało anulowane.");
        if (order.Products.Count > 0)
        {
            Console.WriteLine("Środki zostały zwrócone klientowi.");
        }
        order.ClearOrder();
    }

    public void ShowOrderDetails(Order order)
    {
        Console.WriteLine("Zamówienie opłacone, czeka na spakowanie.");
    }
}

public class ShippedState : IOrderState
{
    public void AddProduct(Order order, string product)
    {
        Console.WriteLine("Nie można dodać produktów do zamówienia po wysłaniu.");
    }

    public void SubmitOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało już złożone i wysłane.");
    }

    public void ConfirmPayment(Order order)
    {
        Console.WriteLine("Płatność została już potwierdzona.");
    }

    public void PackProduct(Order order, string product)
    {
        Console.WriteLine("Nie można spakować produktu po wysłaniu.");
    }

    public void ShipOrder(Order order)
    {
        Console.WriteLine("Zamówienie zostało już wysłane.");
    }

    public void CancelOrder(Order order)
    {
        Console.WriteLine("Nie można anulować zamówienia po wysłaniu.");
    }

    public void ShowOrderDetails(Order order)
    {
        Console.WriteLine("Zamówienie zostało wysłane.");
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Order order = new Order();

        // Dodawanie produktów
        order.AddProduct("Laptop");
        order.AddProduct("Myszka");
        order.ShowOrderDetails();

        // Złożenie zamówienia
        order.SubmitOrder();

        // Potwierdzenie płatności
        order.ConfirmPayment();

        // Spakowanie produktów
        order.PackProduct("Laptop");
        order.PackProduct("Myszka");
        order.ShowOrderDetails();

        // Wysłanie zamówienia
        order.ShipOrder();
    }
}