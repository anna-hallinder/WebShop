using Moq;
using WebShop.Application.Notifications;
using WebShop.Domain.Entities;

namespace WebShopTests.UnitTests.Notifications;

public class ProductSubjectTests
{
    [Fact]
    public void Attach_Should_Register_Observer()
    {
        // Arrange
        var mockObserver = new Mock<INotificationObserver>();
        var productSubject = new ProductSubject(new List<INotificationObserver>());

        // Act
        productSubject.Attach(mockObserver.Object);

        // Assert
        var testProduct = new ProductEntity { Name = "Test Product" };
        productSubject.Notify(testProduct);

        mockObserver.Verify(o => o.Update(It.Is<ProductEntity>(p => p.Name == "Test Product")), Times.Once);
    }

    [Fact]
    public void Notify_Should_Call_Update_On_Observer()
    {
        // Arrange
        var mockObserver = new Mock<INotificationObserver>();
        var productSubject = new ProductSubject(new List<INotificationObserver> { mockObserver.Object });

        var testProduct = new ProductEntity { Name = "Test Product" };

        // Act
        productSubject.Notify(testProduct);

        // Assert
        mockObserver.Verify(o => o.Update(It.Is<ProductEntity>(p => p.Name == "Test Product")), Times.Once);
    }
}