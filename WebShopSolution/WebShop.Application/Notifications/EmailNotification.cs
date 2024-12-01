using WebShop.Domain.Entities;

namespace WebShop.Application.Notifications
{
    public class EmailNotification : INotificationObserver
    {

        public void Update(ProductEntity product)
        {
            if (product == null)
            {
                Console.WriteLine("Email Notification: No product details provided.");
                return;
            }
            Console.WriteLine($"Email Notification: New product added - {product.Id} {product.Name}");
        }
    }
}