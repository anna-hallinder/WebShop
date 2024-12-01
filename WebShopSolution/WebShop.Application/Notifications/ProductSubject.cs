using WebShop.Domain.Entities;

namespace WebShop.Application.Notifications
{
    // Subject som håller reda på observatörer och notifierar dem
    public class ProductSubject
    {
        private readonly List<INotificationObserver> _observers = new List<INotificationObserver>();

        public ProductSubject()
        {
        }

        public ProductSubject(IEnumerable<INotificationObserver> observers)
        {
            _observers.AddRange(observers);
            Console.WriteLine($"Observers registered: {_observers.Count}");
        }

        public void Attach(INotificationObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(INotificationObserver observer)
        {
            // Ta bort en observatör
            _observers.Remove(observer);
        }

        public virtual void Notify(ProductEntity product)
        {
            foreach (var observer in _observers)
            {
                observer.Update(product);
            }
        }
    }
}