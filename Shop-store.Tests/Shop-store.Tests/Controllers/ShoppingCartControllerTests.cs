using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shop_store.Controllers;
using Shop_store.Controllersc.SwaggerDoc;
using Shop_store.Data;
using Shop_store.Models;
using Shop_store.Services;
using Xunit;

namespace Shop_store.Tests.Controllers
{
    public class ShoppingCartControllerTests
    {


        [Fact]
        public async Task GetCartItems_ReturnsOkResult()
        {
       
            var shoppingCartServiceMock = new Mock<IShoppingCartService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<AccountController>>();
            var controller = new ShoppingCartController(shoppingCartServiceMock.Object, userServiceMock.Object, loggerMock.Object);

            var serviceResult = new ServiceResult { Success = true };
            shoppingCartServiceMock.Setup(x => x.GetCartItems(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ShoppingCartItemDto>()); 


            var result = await controller.GetCartItems();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsType<List<ShoppingCartItemDto>>(okResult.Value);
            Assert.Equal(0, items.Count); 
        }

        [Fact]
        public async Task AddToCart_WithValidModel_ReturnsOkResult()
        {

            var shoppingCartServiceMock = new Mock<IShoppingCartService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<AccountController>>();
            var dbContextMock = new Mock<ApplicationDbContext>();
            var controller = new ShoppingCartController(shoppingCartServiceMock.Object, userServiceMock.Object, loggerMock.Object);

            var userId = "109438982097685230115";
            controller.ModelState.Clear(); 

            var itemDto = new ShoppingCartItemDto
            {
                UserId = userId,
                AddedAt = DateTime.Now
            };


            var serviceResult = new ServiceResult { Success = true };
            shoppingCartServiceMock.Setup(x => x.AddToCart(It.IsAny<string>(), It.IsAny<ShoppingCartItemDto>()))
                .ReturnsAsync(serviceResult);

            var result = await controller.AddToCart(itemDto);
            Assert.IsType<OkObjectResult>(result); 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value; 
            Assert.NotNull(value);
            var stringValue = value.GetType().GetProperty("Message")?.GetValue(value);

            Assert.IsType<string>(stringValue);
            Assert.Equal("Item added to the cart successfully", stringValue);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsOkResult()
        {
          
            var shoppingCartServiceMock = new Mock<IShoppingCartService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<AccountController>>();
            var controller = new ShoppingCartController(shoppingCartServiceMock.Object, userServiceMock.Object, loggerMock.Object);

            var userId = "109438982097685230115";

 
            var serviceResult = new ServiceResult { Success = true };
            shoppingCartServiceMock.Setup(x => x.RemoveFromCart(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(serviceResult);


            var result = await controller.RemoveFromCart(itemId: 1);

   
            Assert.IsType<OkObjectResult>(result); 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value; 


            Assert.NotNull(value);
            var stringValue = value.GetType().GetProperty("Message")?.GetValue(value);

            Assert.IsType<string>(stringValue);
            Assert.Equal("Item removed from the cart successfully", stringValue);
        }

        [Fact]
        public async Task UpdateCartItem_ReturnsOkResult()
        {
          
            var shoppingCartServiceMock = new Mock<IShoppingCartService>();
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<AccountController>>();
            var controller = new ShoppingCartController(shoppingCartServiceMock.Object, userServiceMock.Object, loggerMock.Object);

            var userId = "109438982097685230115";

       
            var updatedItemDto = new ShoppingCartItemDto
            {

                UserId = userId,
                AddedAt = DateTime.Now
            };


            var serviceResult = new ServiceResult { Success = true };
            shoppingCartServiceMock.Setup(x => x.UpdateCartItem(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ShoppingCartItemDto>()))
                .ReturnsAsync(serviceResult);

            var result = await controller.UpdateCartItem(itemId: 1, updatedItemDto);
            Assert.IsType<OkObjectResult>(result); 

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value; 
            Assert.NotNull(value);

            var stringValue = value.GetType().GetProperty("Message")?.GetValue(value);
            Assert.IsType<string>(stringValue);
            Assert.Equal("Item updated successfully", stringValue);
        }
    }
}