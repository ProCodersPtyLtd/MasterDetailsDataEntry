using MasterDetailsDataEntry.Demo.Database.Model;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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

        [Fact]
        public void CopyListToTest()
        {
            var orderItem1 = new OrderItem { Id = 2, IsMajor = true, ItemName = "pasta" };
            var orderItem2 = new OrderItem { Id = 3, IsMajor = false, ItemName = "keks" };
            var list1 = new List<OrderItem>();
            list1.Add(orderItem1);
            list1.Add(orderItem2);

            var list2 = new List<OrderItem>();
            list1.CopyListTo(list2);

            Assert.Equal(list1.Count, list2.Count);
            Assert.Equal(list1[0].Id, list2[0].Id);
            Assert.Equal(list1[0].ItemName, list2[0].ItemName);
            Assert.Equal(list1[0].IsMajor, list2[0].IsMajor);
            Assert.Equal(list1[1].ItemName, list2[1].ItemName);
            Assert.Equal(list1[1].ItemName, list2[1].ItemName);
            Assert.Equal(list1[1].IsMajor, list2[1].IsMajor);

            list1.Add(new OrderItem());
            Assert.NotEqual(list1.Count, list2.Count);

            list1[0].Id = -1;
            Assert.NotEqual(list1[0].Id, list2[0].Id);
        }
    }
}
