using MasterDetailsDataEntry.Demo.Database.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MasterDetailsDataEntry.Shared;

namespace MasterDetailsDataEntry.Tests.Helpers
{
    public class ObjectClonerTests
    {
        [Fact]
        public void CopyToTest()
        {
            var orderItem = new OrderItem { Id = 2, IsMajor = true, ItemName = "pasta" };
            var orderItemCopy = new OrderItem();
            var copyHash = orderItemCopy.GetHashCode();
            orderItem.CopyTo(orderItemCopy);
            var copyHashAfterCopy = orderItemCopy.GetHashCode();
            Assert.Equal(orderItem.ItemName, orderItemCopy.ItemName);
            Assert.Equal(copyHash, copyHashAfterCopy);
        }

        [Fact]
        public void CanRestorePreviousStateTest()
        {
            var orderItem = new OrderItem { Id = 2, IsMajor = true, ItemName = "pasta", Price = 10.1m };
            var hash = orderItem.GetHashCode();
            var orderItemReserveCopy = orderItem.GetCopy();
            orderItem.Id = 10;
            orderItem.ItemName = "tune";
            orderItem.Price = 0m;
            Assert.Equal(10, orderItem.Id);
            Assert.Equal("tune", orderItem.ItemName);

            orderItemReserveCopy.CopyTo(orderItem);
            var hash2 = orderItem.GetHashCode();
            Assert.Equal(2, orderItem.Id);
            Assert.Equal("pasta", orderItem.ItemName);
            Assert.Equal(10.1m, orderItem.Price);
            Assert.Equal(hash, hash2);
        }
    }
}
