using CoreMapper;
using CoreMapper.Entities;
using CoreMapper.Strategies;
using CoreMapper.Tests.Models;
using ObjectChangeTracking;
using ObjectChangeTracking.CoreMapper;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace CoreMapper.Tests
{
    public class Test
    {
        protected IMapper CreateMapper()
        {
            IEntitySelector entitySelector = new EntitySelector();

            IMapper mapper = new Mapper();
            mapper.RegisterStrategy<SameNameAndTypeStrategy>();
            mapper.RegisterStrategy<EntityToIdStrategry>();
            mapper.RegisterStrategy(new IdToEntityStrategy(entitySelector));
            mapper.RegisterStrategy<CollectionToAddedStrategy>();
            //mapper.MappingStrategies.Add(new CollectionToRemovedStrategy());
            mapper.RegisterInterceptor<ChangeTrackingInterceptor>();

            return mapper;
        }

        [Fact]
        public void EqualProperties()
        {
            ItemA itemA = new ItemA() { Value = "123", Counter = 12 };
            ItemB itemB = new ItemB() { Value = "...", Counter = 0 };

            CreateMapper().Map(itemA, itemB);

            Assert.Equal("123", itemB.Value);
            Assert.Equal(12, itemB.Counter);
        }

        [Fact]
        public void NullableToNonNullable()
        {
            ItemA objA = new ItemA() { IsActive = true };
            ItemB objB = new ItemB() { IsActive = false };

            CreateMapper().Map(objA, objB);
            
            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NullableToNonNullableWithNullValue()
        {
            ItemA objA = new ItemA() { IsActive = null };
            ItemB objB = new ItemB() { IsActive = true };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NonNullableToNullable()
        {
            ItemB objA = new ItemB() { IsActive = true };
            ItemA objB = new ItemA() { IsActive = false };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NonNullableToNullableWithNullValue()
        {
            ItemB objA = new ItemB() { IsActive = true };
            ItemA objB = new ItemA() { IsActive = null };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void ChangeTracking()
        {
            ItemB objA = new ItemB() { Counter = 123 }.AsTrackable();
            objA.IsActive = true;

            ItemA objB = new ItemA() { IsActive = null };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);  //changed
            Assert.True(objB.Counter == 0); //not changed
        }

        [Fact]
        public void ChangeTrackingCollection()
        {
            ItemB objA = new ItemB();
            objA.Items.Add(new SubItem() { ItemB = new ItemB() { Id = 1 } });

            objA = objA.AsTrackable();
            objA.Items.Add(new SubItem() { ItemB = new ItemB() { Id = 3 } });

            UpdateItem updateItem = new UpdateItem();

            CreateMapper().Map(objA, updateItem);

            Assert.True(updateItem.ItemsAdded.Count == 1);  //changed
            Assert.True(updateItem.ItemsAdded[0].ItemBId == 3);
        }

        [Fact]
        public void IdToEntity()
        {
            UpdateItem updateItem = new UpdateItem();
            updateItem.CategoryId = 123;

            ItemA itemA = new ItemA();

            CreateMapper().Map(updateItem, itemA);
            
            Assert.Equal(123, itemA.Category.Id);
        }

        [Fact]
        public void EntityToId()
        {
            ItemA entity = new ItemA();
            entity.Category = new CategoryA() { Id = 123 };
          
            UpdateItem updateItem = new UpdateItem();

            CreateMapper().Map(entity, updateItem);

            Assert.Equal(123, updateItem.CategoryId);
        }

        [Fact]
        public void Collection()
        {
            ItemA objA = new ItemA() { IsActive = true };
            objA.Items.Add(new SubItem() { SortKey = 2 });

            ItemB objB = new ItemB();

            CreateMapper().Map(objA, objB);

            Assert.Single(objB.Items);
            Assert.Equal(2, objB.Items[0].SortKey);
            Assert.True(ReferenceEquals(objA.Items[0], objB.Items[0]) == false);
        }
    }
}
